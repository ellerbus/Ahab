namespace Ahab.Core
{
    /// <summary>
    /// Represents a single buy/sell situation that can
    /// be utilized to track single stock ROI's
    /// </summary>
    public class Signal
    {
        #region Constructors

        public Signal(string stockId)
        {
            StockId = stockId;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        ///	<summary>
        ///	DebuggerDisplay for this object
        ///	</summary>
        private string DebuggerDisplay
        {
            get
            {
                if (IsOpen)
                {
                    return $"[{StockId}, {Buy.Date:MM/dd/yyyy}] IsOpen";
                }

                return $"[{StockId}, {Sell.Date:MM/dd/yyyy} {ReturnOnSell:0%}] IsClosed";

            }
        }

        #endregion

        #region Properties

        ///	<summary>
        ///	
        ///	</summary>
        public string StockId { get; private set; }

        ///	<summary>
        ///	
        ///	</summary>
        public Price Buy { get; set; }

        ///	<summary>
        ///	
        ///	</summary>
        public Price Sell { get; set; }

        ///	<summary>
        ///	
        ///	</summary>
        public Price Market { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen
        {
            get { return Sell == null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsClosed
        {
            get { return !IsOpen; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ReturnOnSell
        {
            get
            {
                if (IsOpen)
                {
                    return double.NaN;
                }

                return (Sell.Close - Buy.Close) / Buy.Close;
            }
        }

        #endregion
    }
}
