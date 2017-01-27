using System;
using System.Collections.Generic;
using System.Linq;

namespace Ahab.Core.Indicators
{
    public class RelativeStrengthIndex : BaseIndicator
    {
        #region Members

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfPeriods"></param>
        public RelativeStrengthIndex(PriceCollection prices, int numberOfPeriods) : base(prices)
        {
            NumberOfPeriods = numberOfPeriods;
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

            if (!TryCache("V", index, out value))
            {
                double gain = GetAverageGain(index);

                double loss = GetAverageLoss(index);

                if (loss == 0)
                {
                    value = 0;
                }
                else if (gain == 0)
                {
                    value = 100;
                }
                else
                {
                    value = 100 - 100 / (1 + gain / loss);
                }

                Cache("V", index, value);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prices">Collection of prices</param>
        /// <param name="index">Index of the current price</param>
        /// <returns></returns>
        public double GetAverageGain(int index)
        {
            double value = 0;

            if (!TryCache("G", index, out value))
            {
                Price price = Prices[index];

                if (index == 0)
                {
                    value = 0;
                }
                else if (index < NumberOfPeriods)
                {
                    IList<Price> window = Prices.GetWindow(index, NumberOfPeriods).ToList();

                    double gains = 0;

                    for (int i = 1; i < window.Count; i++)
                    {
                        Price a = window[i - 1];
                        Price b = window[i];

                        if (b.Close > a.Close)
                        {
                            gains += b.Close - a.Close;
                        }
                    }

                    value = gains / NumberOfPeriods;
                }
                else
                {
                    Price prev = Prices[index - 1];

                    double gain = price.Close > prev.Close ? price.Close - prev.Close : 0;

                    //  perform normal calculation
                    double previous = GetAverageGain(index - 1);

                    value = (previous * (NumberOfPeriods - 1) + gain) / NumberOfPeriods;
                }

                Cache("G", index, value);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prices">Collection of prices</param>
        /// <param name="index">Index of the current price</param>
        /// <returns></returns>
        public double GetAverageLoss(int index)
        {
            double value = 0;

            if (!TryCache("L", index, out value))
            {
                Price price = Prices[index];

                if (index == 0)
                {
                    value = 0;
                }
                else if (index < NumberOfPeriods)
                {
                    IList<Price> window = Prices.GetWindow(index, NumberOfPeriods).ToList();

                    double losses = 0;

                    for (int i = 1; i < window.Count; i++)
                    {
                        Price a = window[i - 1];
                        Price b = window[i];

                        if (b.Close < a.Close)
                        {
                            losses += b.Close - a.Close;
                        }
                    }

                    value = Math.Abs(losses) / NumberOfPeriods;
                }
                else
                {
                    Price prev = Prices[index - 1];

                    double loss = Math.Abs(price.Close < prev.Close ? price.Close - prev.Close : 0);

                    //  perform normal calculation
                    double previous = GetAverageLoss(index - 1);

                    value = (previous * (NumberOfPeriods - 1) + loss) / NumberOfPeriods;
                }

                Cache("L", index, Math.Abs(value));
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return $"RSI({NumberOfPeriods})"; } }

        /// <summary>
        /// The number of periods to divide by
        /// </summary>
        public int NumberOfPeriods { get; private set; }

        #endregion
    }
}
