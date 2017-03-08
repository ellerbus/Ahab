using System.Collections.Generic;
using System.Linq;

namespace Ahab.Core
{
    public sealed class PortfolioSimulator
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public PortfolioSummary RunSimulation(IPortfolioModel model)
        {
            Portfolio portfolio = new Portfolio(model);

            IList<Signal> signals = model.FindSignals().ToList();

            TradeSignals(model, portfolio, signals);

            return new PortfolioSummary(model, portfolio, signals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        private void TradeSignals(IPortfolioModel model, Portfolio portfolio, IEnumerable<Signal> signals)
        {
            IList<Transaction> transactions = BuildTransactions(signals)
                .OrderBy(x => x.Date)
                .ToList();

            IList<Transaction> buys = new List<Transaction>();

            foreach (Transaction trx in transactions)
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
                    Transaction buy = buys.FirstOrDefault(x => x.Type == TransactionTypes.Buy && x.Signal == trx.Signal);

                    if (buy != null && buy.AppliedToPortfolio)
                    {
                        trx.Shares = buy.Shares;

                        portfolio.ApplyTransaction(trx);

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
    }
}
