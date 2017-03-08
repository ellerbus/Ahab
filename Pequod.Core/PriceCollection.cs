using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Augment;

namespace Ahab.Core
{
    public class PriceCollection : KeyedCollection<DateTime, Price>
    {
        #region Members

        private readonly object _lock = new object();

        #endregion

        #region Constructors

        public PriceCollection(string stockId)
        {
            StockId = stockId;
        }

        public PriceCollection(string stockId, IEnumerable<Price> prices) : this(stockId)
        {
            foreach (Price price in prices.OrderBy(x => x.Date))
            {
                Add(price);
            }
        }

        #endregion

        #region ToString/DebuggerDisplay

        ///	<summary>
        ///	DebuggerDisplay for this object
        ///	</summary>
        private string DebuggerDisplay
        {
            get { return "Count={0}".FormatArgs(Count); }
        }

        #endregion

        #region Collection Methods

        protected override DateTime GetKeyForItem(Price item)
        {
            return item.Date;
        }

        protected override void SetItem(int index, Price item)
        {
            base.SetItem(index, item);

            Update(item);
        }

        protected override void InsertItem(int index, Price item)
        {
            base.InsertItem(index, item);

            Update(item);
        }

        private void Update(Price item)
        {
            item.StockId = StockId;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool ContainsDate(DateTime dt)
        {
            if (dt < MinDate)
            {
                return false;
            }

            if (dt > MaxDate)
            {
                return false;
            }

            return Contains(dt);
        }

        /// <summary>
        /// Based on the <see cref="CurrentIndex"/> and a given lookback
        /// this will return a window of length lookback
        /// </summary>
        /// <param name="endIndex">The ending (inclusive) index</param>
        /// <param name="lookback"></param>
        /// <returns></returns>
        public IEnumerable<Price> GetWindow(int endIndex, int lookback)
        {
            int start = Math.Max(endIndex - lookback + 1, 0);

            for (int i = start; i <= endIndex; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Adjust prices for split
        /// </summary>
        /// <param name="split"></param>
        public void AddSplit(Split split)
        {
            lock (_lock)
            {
                if (split.Date.IsBetween(MinDate, MaxDate))
                {
                    //  ok where does it fit in
                    foreach (Price p in this.Where(x => x.Date < split.Date))
                    {
                        p.MakeAdjustments(split);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public Price FindOrCreate(DateTime date)
        {
            lock (_lock)
            {
                date = date.Date;

                if (Contains(date))
                {
                    return this[date];
                }

                //  insert at the appropriate index
                Price price = new Price { Date = date };

                if (Count == 0)
                {
                    Insert(0, price);
                }
                else
                {
                    int index = 0;

                    for (int i = Count - 1; i >= 0; i--)
                    {
                        if (this[i].Date < date)
                        {
                            index = i + 1;
                            break;
                        }
                    }

                    Insert(index, price);
                }

                return price;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string StockId { get; private set; }

        /// <summary>
        /// Get the Date of the first Price (otherwise DateTime.MaxValue)
        /// </summary>
        public DateTime MinDate
        {
            get
            {
                if (Count == 0)
                {
                    return DateTime.MaxValue;
                }

                return this.First().Date;
            }
        }

        /// <summary>
        /// Get the Date of the last Price (otherwise DateTime.MinValue)
        /// </summary>
        public DateTime MaxDate
        {
            get
            {
                if (Count == 0)
                {
                    return DateTime.MinValue;
                }

                return this.Last().Date;
            }
        }

        #endregion
    }
}
