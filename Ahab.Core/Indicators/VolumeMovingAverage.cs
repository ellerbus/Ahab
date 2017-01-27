using System.Collections.Generic;
using System.Linq;

namespace Ahab.Core.Indicators
{
    public class VolumeMovingAverage : BaseIndicator
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfPeriods"></param>
        public VolumeMovingAverage(PriceCollection prices, int numberOfPeriods) : base(prices)
        {
            NumberOfPeriods = numberOfPeriods;
        }

        #endregion

        #region Methods

        public double GetValue(int index)
        {
            double value = 0;

            if (!TryCache("VMA", index, out value))
            {
                IList<Price> window = Prices.GetWindow(index, NumberOfPeriods).ToList();

                value = window.Select(x => x.Volume).Average();

                Cache("VMA", index, value);
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return $"VMA({NumberOfPeriods})"; } }

        /// <summary>
        /// The number of periods to divide by
        /// </summary>
        public int NumberOfPeriods { get; private set; }

        #endregion
    }
}
