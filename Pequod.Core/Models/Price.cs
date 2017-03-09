using System;
using System.Diagnostics;

namespace Pequod.Core.Models
{
    /// <summary>
    /// The closing price of a Ticker
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public class Price
    {
        #region Methods

        private string DebuggerDisplay()
        {
            string s = $"{GetType().Name} Date={Date:MM/dd/yyyy} Close={Close:C2}";

            if (IsAdjusted)
            {
                s += " (Adjusted)";
            }

            return s;
        }

        public override string ToString()
        {
            return DebuggerDisplay();
        }

        /// <summary>
        /// Adjusts all price data based on the <see cref="Split"/>
        /// </summary>
        public void MakeAdjustments(Split split)
        {
            if (AdjustmentMultiplier == 0)
            {
                AdjustmentMultiplier = split.Adjustment;
            }
            else
            {
                AdjustmentMultiplier *= split.Adjustment;
            }

            Adjust();
        }

        /// <summary>
        /// Adjusts all price data based on original adjusted price
        /// (ie. Open=AdjustmentMultiplier*Open)
        /// </summary>
        public void MakeAdjustments()
        {
            if (!IsAdjusted && AdjustedClose > 0 && Close > 0)
            {
                AdjustmentMultiplier = AdjustedClose / Close;

                Adjust();
            }
        }

        private void Adjust()
        {
            if (AdjustmentMultiplier != 1)
            {
                Open *= AdjustmentMultiplier;
                High *= AdjustmentMultiplier;
                Low *= AdjustmentMultiplier;
                Close *= AdjustmentMultiplier;
            }

            IsAdjusted = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// This prices Stock ID
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Gets / Sets the price date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets / Sets the price open
        /// </summary>
        public double Open { get; set; } = double.NaN;

        /// <summary>
        /// Gets / Sets the price high
        /// </summary>
        public double High { get; set; } = double.NaN;

        /// <summary>
        /// Gets / Sets the price low
        /// </summary>
        public double Low { get; set; } = double.NaN;

        /// <summary>
        /// Gets / Sets the price close
        /// </summary>
        public double Close { get; set; } = double.NaN;

        /// <summary>
        /// Gets / Sets the price adjusted close
        /// </summary>
        public double AdjustedClose { get; set; } = 0;

        /// <summary>
        /// Gets / Sets the days volume
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// Gets the value used to convert (or make adjustments)
        /// (m = AdjustedClose / Close)
        /// </summary>
        public double AdjustmentMultiplier { get; private set; }

        /// <summary>
        /// Whether or not Open,High,Low,Close were adjusted
        /// using a multipler based on AdjustedClose/Close
        /// (ie. Open=m*Open, High=m*High)
        /// </summary>
        public bool IsAdjusted { get; private set; }

        #endregion
    }
}
