using System.Collections.Generic;

namespace Ahab.Core.Indicators
{
    public abstract class BaseIndicator
    {
        private IDictionary<string, double> _cache = new Dictionary<string, double>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prices"></param>
        protected BaseIndicator(PriceCollection prices)
        {
            Prices = prices;
        }

        protected void Cache(string name, int index, double value)
        {
            string key = $"{name}#{index}";

            _cache.Add(key, value);
        }

        protected bool TryCache(string name, int index, out double value)
        {
            string key = $"{name}#{index}";

            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        protected PriceCollection Prices { get; private set; }

        /// <summary>
        /// Name of short description of this indicator (ie. SMA(30))
        /// </summary>
        public abstract string Name { get; }
    }
}
