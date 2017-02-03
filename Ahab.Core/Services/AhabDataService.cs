using System;
using System.Collections.Generic;

namespace Ahab.Core.Services
{
    public interface IAhabDataService
    {
        /// <summary>
        /// Gets the daily historical prices for a given stock id and date range
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<Price> GetDailyHistoricalPrices(string stockId, DateTime start, DateTime end);

        /// <summary>
        /// Gets a list of SP500 <see cref="Symbol"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetConstituents();

        /// <summary>
        /// Gets a list of all available
        /// </summary>
        /// <returns></returns>
        IEnumerable<Symbol> GetAllSymbols();
    }

    public class AhabDataService : IAhabDataService
    {
        #region Members

        private IDownloaderService _downloader;

        #endregion

        #region Constructors

        public AhabDataService() : this(new DownloaderService())
        {

        }

        public AhabDataService(IDownloaderService downloader)
        {
            _downloader = downloader;
        }

        #endregion

        #region IDataService Impl

        public IEnumerable<Symbol> GetAllSymbols()
        {
            NasdaqSymbolService nasdaq = new NasdaqSymbolService(_downloader);

            return nasdaq.GetAllSymbols();
        }

        public IEnumerable<string> GetConstituents()
        {
            Sp500ConstituentService svc = new Sp500ConstituentService(_downloader);

            return svc.GetConstituents();
        }

        public IEnumerable<Price> GetDailyHistoricalPrices(string stockId, DateTime start, DateTime end)
        {
            YahooPriceService svc = new YahooPriceService(_downloader);

            return svc.GetDailyHistoricalPrices(stockId, start, end);
        }

        #endregion
    }
}
