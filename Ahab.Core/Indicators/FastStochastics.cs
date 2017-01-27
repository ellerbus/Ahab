using System.Collections.Generic;
using System.Linq;

namespace Ahab.Core.Indicators
{
    public class FastStochastics : BaseIndicator
    {
        #region Members

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfPeriods"></param>
        public FastStochastics(PriceCollection prices, int numberOfPeriodsK, int numberOfPeriodsD) : base(prices)
        {
            NumberOfPeriodsK = numberOfPeriodsK;
            NumberOfPeriodsD = numberOfPeriodsD;
        }

        #endregion

        #region Methods

        /// <summary>
        /// %K
        /// </summary>
        /// <param name="prices">Collection of prices</param>
        /// <param name="index">Index of the current price</param>
        /// <returns></returns>
        public double GetValue(int index)
        {
            double value = 0;

            if (!TryCache("%K", index, out value))
            {
                Price price = Prices[index];

                if (index >= 0)
                {
                    IList<Price> window = Prices.GetWindow(index, NumberOfPeriodsK).ToList();

                    double high = window.Select(x => x.High).Max();

                    double low = window.Select(x => x.Low).Min();

                    value = (price.Close - low) / (high - low);

                    value *= 100;

                    Cache("%K", index, value);
                }
            }

            return value;
        }

        /// <summary>
        /// %D
        /// </summary>
        /// <param name="prices">Collection of prices</param>
        /// <param name="index">Index of the current price</param>
        /// <returns></returns>
        public double GetSignal(int index)
        {
            double value = 0;

            if (!TryCache("%D", index, out value))
            {
                Price price = Prices[index];

                if (index >= 0)
                {
                    for (int i = 0; i < NumberOfPeriodsD; i++)
                    {
                        value += GetValue(index - i);
                    }

                    Cache("%D", index, value / NumberOfPeriodsD);
                }
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return $"STO({NumberOfPeriodsK})"; } }

        /// <summary>
        /// The number of periods for K
        /// </summary>
        public int NumberOfPeriodsK { get; private set; }

        /// <summary>
        /// The number of periods for D
        /// </summary>
        public int NumberOfPeriodsD { get; private set; }

        #endregion
    }
}
