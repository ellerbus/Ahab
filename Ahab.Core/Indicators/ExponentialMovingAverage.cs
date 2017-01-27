using System.Linq;

namespace Ahab.Core.Indicators
{
    public class ExponentialMovingAverage : BaseIndicator
    {
        #region Members

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfPeriods"></param>
        public ExponentialMovingAverage(PriceCollection prices, int numberOfPeriods) : base(prices)
        {
            NumberOfPeriods = numberOfPeriods;

            Multiplier = 2 / (NumberOfPeriods + 1.0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prices">Collection of prices</param>
        /// <param name="index">Index of the current price</param>
        /// <returns></returns>
        public double GetValue(int index)
        {
            double value = 0;

            if (!TryCache("EMA", index, out value))
            {
                Price price = Prices[index];

                if (index == 0)
                {
                    //  use the close as the EMA
                    value = price.Close;
                }
                else if (index < NumberOfPeriods)
                {
                    //  use the SMA as the EMA until we have the number of periods
                    value = Prices.GetWindow(index, NumberOfPeriods).Select(x => x.Close).Average();
                }
                else
                {
                    //  perform normal calculation
                    double previous = GetValue(index - 1);

                    value = (price.Close - previous) * Multiplier + previous;
                }

                Cache("EMA", index, value);
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return $"EMA({NumberOfPeriods})"; } }

        /// <summary>
        /// The number of periods to divide by
        /// </summary>
        public int NumberOfPeriods { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private double Multiplier { get; set; }

        #endregion
    }
}
