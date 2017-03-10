using System;
using System.Collections.Generic;

namespace Pequod.Core.PortfolioSimulation
{
    /// <summary>
    /// Tracks the money of a portfolio simulation
    /// </summary>
    public sealed class PortfolioLedger
    {
        #region Constructors

        internal PortfolioLedger(IPortfolioModel model)
        {
            Balance = model.StartingBalance;

            Commission = model.Commission;
        }

        #endregion

        #region Methods

        internal void ApplyTransaction(PortfolioTransaction trx)
        {
            trx.Commission = Commission;

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current portfolio balance
        /// </summary>
        public double Balance { get; private set; }

        /// <summary>
        /// The commission cost if any per transaction (default = 8.95)
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<PortfolioTransaction> Transactions { get; } = new List<PortfolioTransaction>();

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, int> SharesOwned { get; } = new Dictionary<string, int>();

        #endregion
    }
}
