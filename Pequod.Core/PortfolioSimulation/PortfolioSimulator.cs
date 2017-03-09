using System.Collections.Generic;
using System.Linq;
using Pequod.Core.Models;

namespace Pequod.Core.PortfolioSimulation
{
    public sealed class PortfolioSimulator
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public PortfolioSummary RunSimulation(IPortfolioModel model)
        {
            PortfolioLedger portfolio = new PortfolioLedger(model);

            IList<Signal> signals = model.FindSignals().ToList();

            TradeSignals(model, portfolio, signals);

            return new PortfolioSummary(model, portfolio, signals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        private void TradeSignals(IPortfolioModel model, PortfolioLedger portfolio, IEnumerable<Signal> signals)
        {
            IList<PortfolioTransaction> transactions = BuildTransactions(signals)
                .OrderBy(x => x.Date)
                .ToList();

            IList<PortfolioTransaction> buys = new List<PortfolioTransaction>();

            foreach (PortfolioTransaction trx in transactions)
            {
                if (trx.Type == TransactionTypes.Buy)
                {
                    int shares = model.GetSharesToPurchase(trx.Signal.Buy, portfolio.Balance);

                    if (shares > 0)
                    {
                        trx.Shares = shares;

                        portfolio.ApplyTransaction(trx);

                        buys.Add(trx);
                    }
                }
                else if (buys.Count > 0)
                {
                    PortfolioTransaction buy = buys.FirstOrDefault(x => x.Type == TransactionTypes.Buy && x.Signal == trx.Signal);

                    if (buy != null && buy.AppliedToPortfolio)
                    {
                        trx.Shares = buy.Shares;

                        portfolio.ApplyTransaction(trx);

                        buys.Remove(buy);
                    }
                }
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
    }
}
