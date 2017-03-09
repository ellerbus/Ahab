namespace Pequod.Core.Models
{
    /// <summary>
    /// Represents a single buy/sell situation that can
    /// be utilized to track single stock ROI's
    /// </summary>
    public class Signal
    {
        #region Constructors

        public Signal(string ticker)
        {
            Ticker = ticker;
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
                    return $"[{Ticker}, {Buy.Date:MM/dd/yyyy}] IsOpen";
                }

                return $"[{Ticker}, {Sell.Date:MM/dd/yyyy} {ReturnOnSell:0%}] IsClosed";

            }
        }

        #endregion

        #region Properties

        ///	<summary>
        ///	
        ///	</summary>
        public string Ticker { get; private set; }

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
            get { return Sell == null || double.IsNaN(Sell.Close); }
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

        /// <summary>
        /// 
        /// </summary>
        public double ReturnOnMarket
        {
            get
            {
                if (IsClosed)
                {
                    return double.NaN;
                }

                return (Market.Close - Buy.Close) / Buy.Close;
            }
        }

        #endregion
    }
}
