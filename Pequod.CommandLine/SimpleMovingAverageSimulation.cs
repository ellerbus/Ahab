//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Ahab.Core;
//using Ahab.Core.Indicators;
//using Ahab.Core.Services;
//using Augment;

//namespace Ahab.CommandLine
//{
//    class SimpleMovingAverageSimulation : IPortfolioModel
//    {
//        #region Members

//        private IDataService _dataService;
//        private int _windowSize = 15;

//        #endregion

//        #region Constructors

//        public SimpleMovingAverageSimulation(DateTime startingDate)
//            : base(startingDate)
//        {
//            _dataService = new DataService();
//        }

//        #endregion

//        #region FindSignals

//        protected override IEnumerable<Signal> FindSignals()
//        {
//            Range<DateTime> dateRange = new Range<DateTime>(StartingDate, EndingDate);

//            HashSet<string> sp500 = new HashSet<string>(_dataService.GetConstituents(), StringComparer.OrdinalIgnoreCase);

//            SymbolCollection symbols = new SymbolCollection(_dataService.GetAllSymbols());


//            foreach (Symbol symbol in symbols.Where(s => sp500.Contains(s.StockId)))
//            {
//                log.Debug($"Analyzing StockId=[{symbol.StockId}]");

//                List<Signal> signals = new List<Signal>();

//                Prices prices = null;

//                try
//                {
//                    prices = GetPricesFor(symbol);
//                }
//                catch
//                {
//                    continue;
//                }

//                //  skip enough to calculate technicals
//                for (int i = 30; i < prices.Count; i++)
//                {
//                    Price price = prices[i];

//                    if (dateRange.Contains(price.Date))
//                    {
//                        if (HasBuySignal(prices, i))
//                        {
//                            signals.Add(CreateSignal(price));
//                        }
//                        else if (HasSellSignal(prices, i))
//                        {
//                            Signal bought = signals.FirstOrDefault(x => x.StockId == prices[i].StockId && x.IsOpen);

//                            if (bought != null)
//                            {
//                                bought.Sell = price;

//                                yield return bought;

//                                signals.Remove(bought);
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        private Signal CreateSignal(Price price)
//        {
//            Signal signal = new Signal(price.StockId)
//            {
//                Buy = price
//            };

//            return signal;
//        }

//        private bool HasBuySignal(Prices prices, int i)
//        {
//            if (!SimpleMovingAverageCrossesAbove(prices, i))
//            {
//                return false;
//            }

//            //if (!StochasticsCrossesAbove(prices, i))
//            //{
//            //    return false;
//            //}

//            //if (!MacdCrossesAbove(prices, i))
//            //{
//            //    return false;
//            //}

//            return true;
//        }

//        private bool HasSellSignal(Prices prices, int i)
//        {
//            if (!SimpleMovingAverageCrossesBelow(prices, i))
//            {
//                return false;
//            }

//            //if (!StochasticsCrossesBelow(prices, i))
//            //{
//            //    return false;
//            //}

//            //if (!MacdCrossesBelow(prices, i))
//            //{
//            //    return false;
//            //}

//            return true;
//        }

//        private bool SimpleMovingAverageCrossesAbove(Prices prices, int i)
//        {
//            for (int x = 1; x < _windowSize; x++)
//            {
//                int index = x + 1 + i - _windowSize;

//                if (index >= 1)
//                {
//                    Price prev = prices[index - 1];
//                    Price now = prices[index];

//                    //if (prev.Close < prices.SimpleMovingAverage.GetValue(index - 1))
//                    {
//                        if (now.Close > prices.SimpleMovingAverage.GetValue(index))
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }

//            return false;
//        }

//        private bool SimpleMovingAverageCrossesBelow(Prices prices, int i)
//        {
//            for (int x = 1; x < _windowSize; x++)
//            {
//                int index = x + 1 + i - _windowSize;

//                if (index >= 1)
//                {
//                    Price prev = prices[index - 1];
//                    Price now = prices[index];

//                    //if (prev.Close > prices.SimpleMovingAverage.GetValue(index - 1))
//                    {
//                        if (now.Close < prices.SimpleMovingAverage.GetValue(index))
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }

//            return false;
//        }

//        private bool StochasticsCrossesAbove(Prices prices, int i)
//        {
//            for (int x = 1; x < _windowSize; x++)
//            {
//                int index = x + 1 + i - _windowSize;

//                if (index >= 1)
//                {
//                    double prvK = prices.FastStochastics.GetValue(index);
//                    double prvD = prices.FastStochastics.GetSignal(index);

//                    double nowK = prices.FastStochastics.GetValue(index);
//                    double nowD = prices.FastStochastics.GetSignal(index);

//                    if (nowK < 20 && nowK > nowD)
//                    {
//                        if (prvK < prvD)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }

//            return false;
//        }

//        private bool StochasticsCrossesBelow(Prices prices, int i)
//        {
//            for (int x = 1; x < _windowSize; x++)
//            {
//                int index = x + 1 + i - _windowSize;

//                if (index >= 1)
//                {
//                    double prvK = prices.FastStochastics.GetValue(index);
//                    double prvD = prices.FastStochastics.GetSignal(index);

//                    double nowK = prices.FastStochastics.GetValue(index);
//                    double nowD = prices.FastStochastics.GetSignal(index);

//                    if (nowK > 80 && nowK < nowD)
//                    {
//                        if (prvK > prvD)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }

//            return false;
//        }

//        private bool MacdCrossesAbove(Prices prices, int i)
//        {
//            for (int x = 1; x < _windowSize; x++)
//            {
//                int index = x + 1 + i - _windowSize;

//                if (index >= 1)
//                {
//                    double prvHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index - 1);
//                    double nowHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index);

//                    if (prvHistogram < 0 && nowHistogram > 0)
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        private bool MacdCrossesBelow(Prices prices, int i)
//        {
//            for (int x = 1; x < _windowSize; x++)
//            {
//                int index = x + 1 + i - _windowSize;

//                if (index >= 1)
//                {
//                    double prvHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index - 1);
//                    double nowHistogram = prices.MovingAverageConvergenceDivergence.GetHistogram(index);

//                    if (prvHistogram > 0 && nowHistogram < 0)
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        #endregion

//        #region GetSharesToPurchase Methods

//        protected override int GetSharesToPurchase(Price buy)
//        {
//            double amount = Math.Min(Portfolio.Balance, 1000);

//            double shares = Math.Floor(amount / buy.Close);

//            double purchase = shares * buy.Close + Portfolio.Commission;

//            double reserve = 1500;

//            if (shares > 0 && Portfolio.Balance - purchase > reserve)
//            {
//                return (int)shares;
//            }

//            return 0;
//        }

//        #endregion

//        #region Methods

//        private Prices GetPricesFor(Symbol symbol)
//        {
//            IEnumerable<Price> data = _dataService.GetDailyHistoricalPrices(symbol.StockId, StartingDate, EndingDate);

//            Prices prices = new Prices(symbol.StockId, data);

//            return prices;
//        }

//        #endregion

//        #region Price Collection Class

//        class Prices : PriceCollection
//        {
//            #region Constructors

//            public Prices(string stockId, IEnumerable<Price> prices) : base(stockId)
//            {
//                foreach (Price price in prices.OrderBy(x => x.Date))
//                {
//                    Add(price);
//                }

//                SimpleMovingAverage = new SimpleMovingAverage(this, 200);

//                MovingAverageConvergenceDivergence = new MovingAverageConvergenceDivergence(this, 12, 26, 9);

//                FastStochastics = new FastStochastics(this, 14, 5);
//            }

//            #endregion

//            #region Indicators

//            public SimpleMovingAverage SimpleMovingAverage { get; private set; }

//            public MovingAverageConvergenceDivergence MovingAverageConvergenceDivergence { get; private set; }

//            public FastStochastics FastStochastics { get; private set; }

//            #endregion
//        }

//        #endregion
//    }
//}
