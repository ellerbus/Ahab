using System;
using System.Diagnostics;

namespace Ahab.Core
{
    public enum TransactionTypes
    {
        /// <summary>
        /// Buying shares
        /// </summary>
        Buy,
        /// <summary>
        /// Selling shares
        /// </summary>
        Sell
    }

    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public class Transaction
    {
        #region Constructors

        public Transaction() { }

        public Transaction(Signal signal, TransactionTypes type)
        {
            Signal = signal;

            Type = type;
        }

        #endregion

        #region Methods

        private string DebuggerDisplay()
        {
            return $"{GetType().Name} [{StockId,-5} {Type,-4} {Date:MM/dd/yyyy}] {Shares,8:N0}x{Amount,13:C2}={Balance,12:C2}";
        }

        public override string ToString()
        {
            return DebuggerDisplay();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The signal that links a buy transaction with
        /// sell transaction
        /// </summary>
        public Signal Signal { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TransactionTypes Type { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private Price PriceObject
        {
            get
            {
                if (Type == TransactionTypes.Buy)
                {
                    return Signal.Buy;
                }

                return Signal.Sell;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StockId { get { return Signal.StockId; } }

        /// <summary>
        /// 
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Shares { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Price { get { return PriceObject.Close; } }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get { return PriceObject.Date; } }

        /// <summary>
        /// The total amount going withdrawn or deposited into
        /// the portfolio
        /// </summary>
        public double Amount
        {
            get
            {
                if (Type == TransactionTypes.Buy)
                {
                    return -(Shares * Price + Commission);
                }

                return Shares * Price - Commission;
            }
        }

        /// <summary>
        /// Balance after amount has posted
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// Whether or not this transaction was applied
        /// to the portfolio, or skipped
        /// </summary>
        public bool AppliedToPortfolio { get; set; }

        #endregion
    }
}
