# Captain Ahab

Captain Ahab is a fictional character in Herman Melville's
Moby-Dick (1851), the monomaniacal captain of the whaling
ship Pequod. On a previous voyage, the white whale Moby
Dick bit off Ahab's leg, leaving him with a prosthesis
made out of whalebone. The whaling voyage of the Pequod
ends up as a hunt for revenge on the whale, as Ahab impels
the crew-members to support his fanatical mission. When
Moby Dick is finally sighted, Ahab's hate robs him of
all caution, and the whale drags Ahab to the bottom of the sea.

---

The start of a simple portfolio simulator for buying and
selling US stocks given various technical analysis rules.
Simulation extends a PortfolioSimulator to customize the
triggers for buy and sell signals as well as implementing
the rules governing when to buy and with how much cash.

```
24461   [INFO  1  ] Model:     SimpleMovingAverageSimulation
24461   [INFO  1  ] Years:     2007 - 2017
24465   [INFO  1  ] Starting:     10,000.00
24466   [INFO  1  ] Portfolio:     6,995.03
24467   [INFO  1  ] ROI:             -30.0%
24467   [INFO  1  ] CAGR:             -3.5%
24468   [INFO  1  ] # of trx:         98
```

```cs
class SimpleMovingAverageSimulation : PortfolioSimulator
{
    private static readonly Container _container = Injector.Register();

    private ISp500ConstituentService _indexService;
    private ISymbolService _symbolService;
    private IPriceService _priceService;
    private int _windowSize = 15;

    public SimpleMovingAverageSimulation(DateTime startingDate)
        : base(startingDate)
    {
        _indexService = _container.GetInstance<ISp500ConstituentService>();

        _symbolService = _container.GetInstance<ISymbolService>();

        _priceService = _container.GetInstance<IPriceService>();
    }

    protected override IEnumerable<Signal> FindSignals()
    {
        Range<DateTime> dateRange = new Range<DateTime>(StartingDate, EndingDate);

        HashSet<string> sp500 = new HashSet<string>(_indexService.GetConstituents(), StringComparer.OrdinalIgnoreCase);

        SymbolCollection symbols = new SymbolCollection(_symbolService.GetAllSymbols());

        foreach (Symbol symbol in symbols.Where(s => sp500.Contains(s.StockId)))
        {
            log.Debug($"Analyzing StockId=[{symbol.StockId}]");

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
        if (!SimpleMovingAverageCrossesAbove(prices, i))
        {
            return false;
        }

        //if (!StochasticsCrossesAbove(prices, i))
        //{
        //    return false;
        //}

        //if (!MacdCrossesAbove(prices, i))
        //{
        //    return false;
        //}

        return true;
    }

    private bool HasSellSignal(Prices prices, int i)
    {
        if (!SimpleMovingAverageCrossesBelow(prices, i))
        {
            return false;
        }

        //if (!StochasticsCrossesBelow(prices, i))
        //{
        //    return false;
        //}

        //if (!MacdCrossesBelow(prices, i))
        //{
        //    return false;
        //}

        return true;
    }

    private bool SimpleMovingAverageCrossesAbove(Prices prices, int i)
    {
        for (int x = 1; x < _windowSize; x++)
        {
            int index = x + 1 + i - _windowSize;

            if (index >= 1)
            {
                Price prev = prices[index - 1];
                Price now = prices[index];

                //if (prev.Close < prices.SimpleMovingAverage.GetValue(index - 1))
                {
                    if (now.Close > prices.SimpleMovingAverage.GetValue(index))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool SimpleMovingAverageCrossesBelow(Prices prices, int i)
    {
        for (int x = 1; x < _windowSize; x++)
        {
            int index = x + 1 + i - _windowSize;

            if (index >= 1)
            {
                Price prev = prices[index - 1];
                Price now = prices[index];

                //if (prev.Close > prices.SimpleMovingAverage.GetValue(index - 1))
                {
                    if (now.Close < prices.SimpleMovingAverage.GetValue(index))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    protected override int GetSharesToPurchase(Price buy)
    {
        double amount = Math.Min(Portfolio.Balance, 1000);

        double shares = Math.Floor(amount / buy.Close);

        double purchase = shares * buy.Close + Portfolio.Commission;

        double reserve = 1500;

        if (shares > 0 && Portfolio.Balance - purchase > reserve)
        {
            return (int)shares;
        }

        return 0;
    }

    private Prices GetPricesFor(Symbol symbol)
    {
        IEnumerable<Price> data = _priceService.GetDailyHistoricalPrices(symbol.StockId, StartingDate, EndingDate);

        Prices prices = new Prices(symbol.StockId, data);

        return prices;
    }

    class Prices : PriceCollection
    {
        public Prices(string stockId, IEnumerable<Price> prices) : base(stockId)
        {
            foreach (Price price in prices.OrderBy(x => x.Date))
            {
                Add(price);
            }

            SimpleMovingAverage = new SimpleMovingAverage(this, 200);
        }

        public SimpleMovingAverage SimpleMovingAverage { get; private set; }
    }
}
```

## Other Points of Interest

- Open Source Lib Inspiration : https://github.com/salmonthinlion/Quandl.NET