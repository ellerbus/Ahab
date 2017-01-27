using System;
using System.Collections.Generic;

namespace Ahab.Core
{
    public class Portfolio
    {
        #region Constructors

        public Portfolio(double startingBalance)
        {
            Balance = startingBalance;

            Commission = 8.95;
        }

        #endregion

        #region Methods

        public void ApplyTransaction(Transaction trx)
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

            if (!SharesOwned.ContainsKey(trx.StockId))
            {
                SharesOwned[trx.StockId] = 0;
            }

            if (trx.Type == TransactionTypes.Buy)
            {
                SharesOwned[trx.StockId] += trx.Shares;
            }
            else if (trx.Type == TransactionTypes.Sell)
            {
                SharesOwned[trx.StockId] += trx.Shares;
            }

            if (SharesOwned[trx.StockId] < 0)
            {
                string msg = $"Invalid Transaction, shares of {trx.StockId} are below 0.\n[{trx.ToString()}]";

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
        public IList<Transaction> Transactions { get; } = new List<Transaction>();

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, int> SharesOwned { get; } = new Dictionary<string, int>();

        #endregion
    }
}
