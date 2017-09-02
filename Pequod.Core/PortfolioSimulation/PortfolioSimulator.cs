using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pequod.Core.Models;

namespace Pequod.Core.PortfolioSimulation
{
    public sealed class PortfolioSimulator
    {
        #region Constructors

        public PortfolioSimulator(ISimulationModel model)
        {
            Model = model;
        }

        #endregion

        #region Methods

        public string SimulationSummary()
        {
            StringBuilder display = new StringBuilder();

            int trxs = Transactions.Count;

            display
                .AppendLine()
                .Append($"Model:       {Model.GetType().Name}").AppendLine()
                .Append($"Years:        {Model.StartingDate.Year} - {Model.EndingDate.Year}").AppendLine()
                .Append($"Starting:    {Model.StartingBalance,12:N2}").AppendLine()
                .Append($"Portfolio:   {Balance,12:N2}").AppendLine()
                .Append($"ROI:         {Return,12:0.0%}").AppendLine()
                .Append($"CAGR:        {Cagr,12:0.0%}").AppendLine()
                .Append($"# Triggers:  {TradedSignals.Count(),9:0}").AppendLine()
                .Append($"# + ROI:     {TradedSignals.Count(x => x.ReturnOnSell > 0),9:0}").AppendLine()
                .Append($"# - ROI:     {TradedSignals.Count(x => x.ReturnOnSell < 0),9:0}").AppendLine();

            return display.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunSimulation()
        {
            RunSimulation(Model.StartingDate, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunSimulation(DateTime portfolioStart, bool allowOpenSignals)
        {
            Balance = Model.StartingBalance;

            TradedSignals.Clear();

            Transactions.Clear();

            SharesOwned.Clear();

            IList<Signal> signals = Model.FindSignals()
                .Where(x => x.Buy.Date >= portfolioStart && (allowOpenSignals || x.IsClosed))
                .ToList();

            TradeSignals(signals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        private void TradeSignals(IList<Signal> signals)
        {
            IList<PortfolioTransaction> transactions = BuildTransactions(signals)
                .OrderBy(x => x.Date)
                .ToList();

            IList<PortfolioTransaction> buys = new List<PortfolioTransaction>();

            foreach (PortfolioTransaction trx in transactions)
            {
                if (trx.Type == TransactionTypes.Buy)
                {
                    int shares = Model.GetSharesToPurchase(trx.Signal.Buy.Close, Balance);

                    if (shares > 0)
                    {
                        trx.Shares = shares;

                        ApplyTransaction(trx);

                        buys.Add(trx);

                        TradedSignals.Add(trx.Signal);
                    }
                }
                else if (buys.Count > 0)
                {
                    PortfolioTransaction buy = buys.FirstOrDefault(x => x.Type == TransactionTypes.Buy && x.Signal == trx.Signal);

                    if (buy != null && buy.AppliedToPortfolio)
                    {
                        trx.Shares = buy.Shares;

                        ApplyTransaction(trx);

                        buys.Remove(buy);
                    }
                }
            }
        }

        private void ApplyTransaction(PortfolioTransaction trx)
        {
            trx.Commission = Model.Commission;

            trx.Balance = Balance + trx.Amount;

            trx.AppliedToPortfolio = true;

            Transactions.Add(trx);

            Balance = trx.Balance;

            if (Balance < 0)
            {
                string msg = $"Invalid Transaction, Portfolio Balance is below 0.\n[{trx.ToString()}]";

                throw new InvalidOperationException(msg);
            }

            if (!SharesOwned.ContainsKey(trx.Symbol))
            {
                SharesOwned[trx.Symbol] = 0;
            }

            if (trx.Type == TransactionTypes.Buy)
            {
                SharesOwned[trx.Symbol] += trx.Shares;
            }
            else if (trx.Type == TransactionTypes.Sell)
            {
                SharesOwned[trx.Symbol] += trx.Shares;
            }

            if (SharesOwned[trx.Symbol] < 0)
            {
                string msg = $"Invalid Transaction, shares of {trx.Symbol} are below 0.\n[{trx.ToString()}]";

                throw new InvalidOperationException(msg);
            }
        }

        private IEnumerable<PortfolioTransaction> BuildTransactions(IEnumerable<Signal> signals)
        {
            foreach (Signal signal in signals)
            {
                yield return new PortfolioTransaction(signal, TransactionTypes.Buy);

                if (signal.IsClosed)
                {
                    yield return new PortfolioTransaction(signal, TransactionTypes.Sell);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The model used
        /// </summary>
        public ISimulationModel Model { get; private set; }

        /// <summary>
        /// The total number of years simulated
        /// </summary>
        public int Years
        {
            get { return Model.EndingDate.Year - Model.StartingDate.Year; }
        }

        /// <summary>
        /// The total portfolio return (profit or loss)
        /// </summary>
        public double Return
        {
            get { return (Balance - Model.StartingBalance) / Model.StartingBalance; }
        }

        /// <summary>
        /// The annualized growth rate per year
        /// </summary>
        public double Cagr
        {
            get { return Math.Pow(Balance / Model.StartingBalance, 1.0 / Years) - 1; }
        }

        /// <summary>
        /// Gets the current portfolio balance
        /// </summary>
        public double Balance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Signal> TradedSignals { get; } = new List<Signal>();

        /// <summary>
        /// 
        /// </summary>
        public List<PortfolioTransaction> Transactions { get; } = new List<PortfolioTransaction>();

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, int> SharesOwned { get; } = new Dictionary<string, int>();

        #endregion
    }
}
