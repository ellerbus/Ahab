using System;
using System.Collections.Generic;
using System.Linq;
using Ahab.Core;
using Ahab.Core.Indicators;
using Ahab.Core.Services;
using Augment;

namespace Ahab.CommandLine
{
    class TechnicalAnalysisPortfolioModel : IPortfolioModel
    {
        #region Members

        private IAhabDataService _dataService;
        private int _windowSize = 3;

        #endregion

        #region Constructors

        public TechnicalAnalysisPortfolioModel()
        {
            _dataService = new AhabDataService();
        }

        #endregion

        #region Forecast

        public IEnumerable<Signal> Forecast()
        {
            yield break;
        }

        #endregion

        #region FindSignals

        public IEnumerable<Signal> FindSignals()
        {
            Range<DateTime> dateRange = new Range<DateTime>(StartingDate, EndingDate);

            HashSet<string> sp500 = new HashSet<string>(_dataService.GetConstituents(), StringComparer.OrdinalIgnoreCase);

            SymbolCollection symbols = new SymbolCollection(_dataService.GetAllSymbols());

            foreach (Symbol symbol in symbols.Where(s => sp500.Contains(s.StockId)))
            {
                List<Signal> signals = new List<Signal>();

                Prices prices = null;

                try
                {
                    prices = GetPricesFor(symbol);
                }
                catch
                {
                    continue;
                }

                //  skip enough to calculate technicals
                for (int i = 30; i < prices.Count; i++)
                {
                    Price price = prices[i];

                    if (dateRange.Contains(price.Date))
                    {
                        if (HasBuySignal(prices, i))
                        {
                            signals.Add(CreateSignal(price));
                        }
                        else if (HasSellSignal(prices, i))
                        {
                            Signal bought = signals.FirstOrDefault(x => x.StockId == prices[i].StockId && x.IsOpen);

                            if (bought != null)
                            {
                                bought.Sell = price;

                                yield return bought;

                                signals.Remove(bought);
                            }
                        }
                    }
                }
            }
        }

        private Signal CreateSignal(Price price)
        {
            Signal signal = new Signal(price.StockId)
            {
                Buy = price
            };

            return signal;
        }

        private bool HasBuySignal(Prices prices, int i)
        {
            if (!SimpleMovingAverageAnyCrossesAbove(prices, i))
            {
                return false;
            }

            if (!StochasticsAnyCrossesAbove(prices, i))
            {
                return false;
            }

            if (!MacdAnyCrossesAbove(prices, i))
            {
                return false;
            }

            return true;
        }

        private bool HasSellSignal(Prices prices, int i)
        {
            if (!SimpleMovingAverageAnyBelow(prices, i))
            {
                return false;
            }

            if (!StochasticsAnyCrossesBelow(prices, i))
            {
                return false;
            }

            if (!MacdAnyCrossesBelow(prices, i))
            {
                return false;
            }

            return true;
        }

        private bool SimpleMovingAverageAnyCrossesAbove(Prices prices, int i)
        {
            for (int x = 0; x < _windowSize; x++)
            {
                int index = x + 1 + i - _windowSize;

                Price prv = prices[index - 1];
                Price now = prices[index];

                if (prv.Close < prices.SimpleMovingAverage.GetValue(index - 1))
                {
                    if (now.Close > prices.SimpleMovingAverage.GetValue(index))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SimpleMovingAverageAnyBelow(Prices prices, int i)
        {
            for (int x = 0; x < _windowSize; x++)
            {
                int index = x + 1 + i - _windowSize;

                if (index >= 1)
                {
                    Price now = prices[index];

                    if (now.Close < prices.SimpleMovingAverage.GetValue(index))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool StochasticsAnyCrossesAbove(Prices prices, int i)
        {
            for (int x = 0; x < _windowSize; x++)
            {
                int index = x + 1 + i - _windowSize;

                double prvK = prices.FastStochastics.GetValue(index - 1);
                double prvD = prices.FastStochastics.GetSignal(index - 1);

                double nowK = prices.FastStochastics.GetValue(index);
                double nowD = prices.FastStochastics.GetSignal(index);

                if (nowK < 20 && nowK > nowD)
                {
                    if (prvK < prvD)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool StochasticsAnyCrossesBelow(Prices prices, int i)
        {
            for (int x = 0; x < _windowSize; x++)
            {
                int index = x + 1 + i - _windowSize;

                double prvK = prices.FastStochastics.GetValue(index - 1);
                double prvD = prices.FastStochastics.GetSignal(index - 1);

                double nowK = prices.FastStochastics.GetValue(index);
                double nowD = prices.FastStochastics.GetSignal(index);

                if (nowK > 80 && nowK < nowD)
                {
                    if (prvK > prvD)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool MacdAnyCrossesAbove(Prices prices, int i)
        {
            for (int x = 0; x < _windowSize; x++)
            {
                int index = x + 1 + i - _windowSize;

                double prvHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index - 1);
                double nowHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index);

                if (prvHistogram < 0 && nowHistogram > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool MacdAnyCrossesBelow(Prices prices, int i)
        {
            for (int x = 0; x < _windowSize; x++)
            {
                int index = x + 1 + i - _windowSize;

                double prvHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index - 1);
                double nowHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index);

                if (prvHistogram > 0 && nowHistogram < 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region GetSharesToPurchase Methods

        public int GetSharesToPurchase(Price buy, double balance)
        {
            double amount = Math.Min(balance, 1000);

            double shares = Math.Floor(amount / buy.Close);

            double purchase = shares * buy.Close + Commission;

            double reserve = 1500;

            if (shares > 0 && balance - purchase > reserve)
            {
                return (int)shares;
            }

            return 0;
        }

        #endregion

        #region Methods

        private Prices GetPricesFor(Symbol symbol)
        {
            IEnumerable<Price> data = _dataService.GetDailyHistoricalPrices(symbol.StockId, StartingDate, EndingDate);

            Prices prices = new Prices(symbol.StockId, data);

            return prices;
        }

        #endregion

        #region Properties


        /// <summary>
        /// Starting balance for this simulation
        /// </summary>
        public double StartingBalance { get { return 10000; } }

        /// <summary>
        /// Commission costs per transaction
        /// </summary>
        public double Commission { get { return 8.95; } }

        /// <summary>
        /// Starting date for this simulation
        /// </summary>
        public DateTime StartingDate { get { return new DateTime(2002, 1, 1); } }

        /// <summary>
        /// Ending date for this simulation (today by default)
        /// </summary>
        public DateTime EndingDate { get { return DateTime.Today; } }

        #endregion

        #region Price Collection Class

        class Prices : PriceCollection
        {
            #region Constructors

            public Prices(string stockId, IEnumerable<Price> prices) : base(stockId)
            {
                foreach (Price price in prices.OrderBy(x => x.Date))
                {
                    Add(price);
                }

                SimpleMovingAverage = new SimpleMovingAverage(this, 30);

                MovingAverageConvergenceDivergence = new MovingAverageConvergenceDivergence(this, 8, 17, 9);

                FastStochastics = new FastStochastics(this, 14, 5);
            }

            #endregion

            #region Indicators

            public SimpleMovingAverage SimpleMovingAverage { get; private set; }

            public MovingAverageConvergenceDivergence MovingAverageConvergenceDivergence { get; private set; }

            public FastStochastics FastStochastics { get; private set; }

            #endregion
        }

        #endregion
    }
}
