using Pequod.Core.Models;

namespace Pequod.Core.Indicators
{
    public class MovingAverageConvergenceDivergence : BaseIndicator
    {
        #region Members

        private ExponentialMovingAverage _fasterMovingAverage;

        private ExponentialMovingAverage _slowerMovingAverage;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public MovingAverageConvergenceDivergence(PriceCollection prices, int fasterMovingAverage, int slowerMovingAverage, int signalNumberOfPeriods) : base(prices)
        {
            _fasterMovingAverage = new ExponentialMovingAverage(prices, fasterMovingAverage);

            _slowerMovingAverage = new ExponentialMovingAverage(prices, slowerMovingAverage);

            SignalNumberOfPeriods = signalNumberOfPeriods;

            SignalMultiplier = 2 / (signalNumberOfPeriods + 1.0);
        }

        #endregion

        #region Methods

        public double GetHistogram(int index)
        {
            double line = GetLine(index);

            double signal = GetSignal(index);

            return line - signal;
        }

        public double GetLine(int index)
        {
            double faster = _fasterMovingAverage.GetValue(index);

            double slower = _slowerMovingAverage.GetValue(index);

            return faster - slower;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetSignal(int index)
        {
            double value = 0;

            if (!TryCache("S", index, out value))
            {
                double line = GetLine(index);

                if (index == 0)
                {
                    value = line;
                }
                else if (index < SignalNumberOfPeriods)
                {
                    double sum = 0;

                    for (int x = 0; x < SignalNumberOfPeriods; x++)
                    {
                        sum += GetLine(index - x);
                    }

                    value = sum / SignalNumberOfPeriods;
                }
                else
                {
                    double previous = GetSignal(index - 1);

                    value = (line - previous) * SignalMultiplier + previous;
                }

                Cache("S", index, value);
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return $"MACD({FasterNumberOfPeriods},{SlowerNumberOfPeriods},{SignalNumberOfPeriods})"; } }

        /// <summary>
        /// 
        /// </summary>
        public int FasterNumberOfPeriods { get { return _fasterMovingAverage.NumberOfPeriods; } }

        /// <summary>
        /// 
        /// </summary>
        public int SlowerNumberOfPeriods { get { return _slowerMovingAverage.NumberOfPeriods; } }

        /// <summary>
        /// 
        /// </summary>
        public int SignalNumberOfPeriods { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private double SignalMultiplier { get; set; }

        #endregion
    }
}
