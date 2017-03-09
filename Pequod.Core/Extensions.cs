using System;
using System.Collections.Generic;
using System.Linq;
using Augment;

namespace Ahab.Core
{
    public static class Extensions
    {
        #region Double Extensions

        /// <summary>
        /// Gets a market cap enum
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static MarketCaps ToMarketCapDescription(this double number)
        {
            if (double.IsNaN(number))
            {
                return MarketCaps.None;
            }
            if (number < 50.Million())
            {
                return MarketCaps.Nano;
            }
            if (number < 300.Million())
            {
                return MarketCaps.Micro;
            }
            if (number < 2.Billion())
            {
                return MarketCaps.Small;
            }
            if (number < 10.Billion())
            {
                return MarketCaps.Mid;
            }
            if (number < 200.Billion())
            {
                return MarketCaps.Large;
            }



            return MarketCaps.Mega;
        }

        public static double Billion(this int number)
        {
            return number * 1e9;
        }

        public static double Million(this int number)
        {
            return number * 1e6;
        }

        public static double StdDeviation(this IEnumerable<double> values)
        {
            double ret = 0;

            int count = values.Count();

            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }

            return ret;
        }

        #endregion
    }
}
