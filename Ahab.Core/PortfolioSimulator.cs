using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Ahab.Core
{
    public abstract class PortfolioSimulator
    {
        #region Member

        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        protected PortfolioSimulator(DateTime startingDate, double startingBalance = 10000)
        {
            StartingBalance = startingBalance;

            StartingDate = startingDate;

            EndingDate = DateTime.Today;

            Portfolio = new Portfolio(startingBalance);
        }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented determines which symbols are a buy/sell
        /// signal
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Signal> FindSignals();

        /// <summary>
        /// Determine the total number of shares to purchase
        /// </summary>
        /// <param name="buy"></param>
        /// <returns>Return less than 1 to ignore the request</returns>
        protected abstract int GetSharesToPurchase(Price buy);

        /// <summary>
        /// 
        /// </summary>
        public void RunSimulation()
        {
            IEnumerable<Signal> signals = FindSignals();

            TradeSignals(signals);

            DisplayPortfolioSummary();
        }

        private void DisplayPortfolioSummary()
        {
            int years = DateTime.Today.Year - StartingDate.Year;

            double roi = (Portfolio.Balance - StartingBalance) / StartingBalance;

            double cagr = Math.Pow(Portfolio.Balance / StartingBalance, 1.0 / years) - 1;

            int trxs = Portfolio.Transactions.Count;

            log.Info($"Model:     {GetType().Name}");
            log.Info($"Starting:  {StartingBalance,12:N2}");
            log.Info($"Portfolio: {Portfolio.Balance,12:N2}");
            log.Info($"ROI:       {roi,12:0.0%}");
            log.Info($"CAGR:      {cagr,12:0.0%}");
            log.Info($"# of trx:  {trxs,9:0}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        protected virtual void TradeSignals(IEnumerable<Signal> signals)
        {
            IList<Transaction> transactions = BuildTransactions(signals)
                .OrderBy(x => x.Date)
                .ToList();

            IList<Transaction> buys = new List<Transaction>();

            foreach (Transaction trx in transactions)
            {
                if (trx.Type == TransactionTypes.Buy)
                {
                    int shares = GetSharesToPurchase(trx.Signal.Buy);

                    if (shares > 0)
                    {
                        trx.Shares = shares;

                        Portfolio.ApplyTransaction(trx);

                        log.Debug($"BUY  {trx}");

                        buys.Add(trx);
                    }
                }
                else if (buys.Count > 0)
                {
                    Transaction buy = buys.FirstOrDefault(x => x.Type == TransactionTypes.Buy && x.Signal == trx.Signal);

                    if (buy != null && buy.AppliedToPortfolio)
                    {
                        trx.Shares = buy.Shares;

                        Portfolio.ApplyTransaction(trx);

                        log.Debug($"SELL {trx} {trx.Signal.ReturnOnSell,7:0%}");

                        buys.Remove(buy);
                    }
                }
            }
        }

        private IEnumerable<Transaction> BuildTransactions(IEnumerable<Signal> signals)
        {
            foreach (Signal signal in signals)
            {
                yield return new Transaction(signal, TransactionTypes.Buy);

                if (signal.IsClosed)
                {
                    yield return new Transaction(signal, TransactionTypes.Sell);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Starting balance for this simulation
        /// </summary>
        public Portfolio Portfolio { get; private set; }

        /// <summary>
        /// Starting balance for this simulation
        /// </summary>
        public double StartingBalance { get; private set; }

        /// <summary>
        /// Starting date for this simulation
        /// </summary>
        public DateTime StartingDate { get; private set; }

        /// <summary>
        /// Ending date for this simulation (today by default)
        /// </summary>
        public DateTime EndingDate { get; set; }

        #endregion
    }
}
