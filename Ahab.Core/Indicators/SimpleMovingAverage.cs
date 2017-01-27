using System.Collections.Generic;
using System.Linq;

namespace Ahab.Core.Indicators
{
    public class SimpleMovingAverage : BaseIndicator
    {
        #region Members

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfPeriods"></param>
        public SimpleMovingAverage(PriceCollection prices, int numberOfPeriods) : base(prices)
        {
            NumberOfPeriods = numberOfPeriods;
        }

        #endregion

        #region Methods

        public double GetValue(int index)
        {
            double value = 0;

            if (!TryCache("SMA", index, out value))
            {
                IList<Price> window = Prices.GetWindow(index, NumberOfPeriods).ToList();

                value = window.Select(x => x.Close).Average();

                Cache("SMA", index, value);
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return $"SMA({NumberOfPeriods})"; } }

        /// <summary>
        /// The number of periods to divide by
        /// </summary>
        public int NumberOfPeriods { get; private set; }

        #endregion
    }
}
