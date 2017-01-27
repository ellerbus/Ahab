namespace Ahab.Core.Indicators
{
    public class SlowStochastics : BaseIndicator
    {
        #region Members

        private FastStochastics _fast;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public SlowStochastics(PriceCollection prices, int numberOfPeriodsK, int numberOfPeriodsD) : base(prices)
        {
            _fast = new FastStochastics(prices, numberOfPeriodsK, numberOfPeriodsD);
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

            Price price = Prices[index];

            if (!TryCache("%K", index, out value))
            {
                if (index >= 0)
                {
                    for (int i = 0; i < NumberOfPeriodsD; i++)
                    {
                        value += _fast.GetValue( index - i);
                    }

                    Cache("%K", index, value / NumberOfPeriodsD);
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
        public override string Name { get { return $"STO:slow({NumberOfPeriodsK})"; } }

        /// <summary>
        /// The number of periods for K
        /// </summary>
        public int NumberOfPeriodsK { get { return _fast.NumberOfPeriodsK; } }

        /// <summary>
        /// The number of periods for D
        /// </summary>
        public int NumberOfPeriodsD { get { return _fast.NumberOfPeriodsD; } }

        #endregion
    }
}
