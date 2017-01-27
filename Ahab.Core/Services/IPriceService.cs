using System;
using System.Collections.Generic;

namespace Ahab.Core.Services
{
    /// <summary>
    /// Represents the interface for getting price history data
    /// </summary>
    public interface IPriceService
    {
        /// <summary>
        /// Gets the daily historical prices for a given stock id and date range
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<Price> GetDailyHistoricalPrices(string stockId, DateTime start, DateTime end);
    }
}
