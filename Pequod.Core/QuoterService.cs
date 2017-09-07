using System;
using System.Collections.Generic;
using System.IO;
using Augment;
using CsvHelper;
using CsvHelper.Configuration;
using Pequod.Core.Models;

namespace Pequod.Core
{
    public interface IQuoterService
    {
        IEnumerable<IndexSymbol> GetSp500Symbols();

        IEnumerable<Price> GetEndOfDayPrices(string symbol, DateTime date);

        IEnumerable<Price> GetEndOfDayPrices(string symbol, DateTime start, DateTime end);

        IEnumerable<Price> GetEndOfDayPrices(IEnumerable<string> symbols);
    }

    public class QuoterService : IQuoterService
    {
        #region Members

        private const string _quandlUrl = "https://www.quandl.com/api/v3";

        private IDownloaderService _downloader;

        #endregion

        #region Constructors

        public QuoterService(IDownloaderService downloader)
        {
            _downloader = downloader;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IndexSymbol> GetSp500Symbols()
        {
            string url = "http://data.okfn.org/data/core/s-and-p-500-companies/r/constituents.csv";

            string content = _downloader.GetStringAsync(url).Result;

            CsvConfiguration cfg = IndexSymbolMap.GetConfiguration();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            return csvReader.GetRecords<IndexSymbol>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<Price> GetEndOfDayPrices(DateTime date)
        {
            string url = GetBaseQuanDlUrl() +
                $"&date={date:yyyy-MM-dd}";

            string content = _downloader.GetStringAsync(url).Result;

            CsvConfiguration cfg = PriceMap.GetConfigurationForQuanDL();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            foreach (Price p in csvReader.GetRecords<Price>())
            {
                p.MakeAdjustments();

                yield return p;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<Price> GetEndOfDayPrices(string symbol, DateTime date)
        {
            string url = GetBaseQuanDlUrl() +
                $"&date={date:yyyy-MM-dd}" +
                $"&ticker={symbol}";

            string content = _downloader.GetStringAsync(url).Result;

            CsvConfiguration cfg = PriceMap.GetConfigurationForQuanDL();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            foreach (Price p in csvReader.GetRecords<Price>())
            {
                p.MakeAdjustments();

                yield return p;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<Price> GetEndOfDayPrices(string symbol, DateTime start, DateTime end)
        {
            string url = GetBaseQuanDlUrl() +
                $"&date.gte={start:yyyy-MM-dd}" +
                $"&date.lte={end:yyyy-MM-dd}" +
                $"&ticker={symbol}";

            string content = _downloader.GetStringAsync(url).Result;

            CsvConfiguration cfg = PriceMap.GetConfigurationForQuanDL();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            foreach (Price p in csvReader.GetRecords<Price>())
            {
                p.MakeAdjustments();

                yield return p;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<Price> GetEndOfDayPrices(IEnumerable<string> symbols)
        {
            string url = GetBaseQuanDlUrl() +
                $"&ticker={symbols.Join(",")}";

            string content = _downloader.GetStringAsync(url).Result;

            CsvConfiguration cfg = PriceMap.GetConfigurationForQuanDL();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            foreach (Price p in csvReader.GetRecords<Price>())
            {
                p.MakeAdjustments();

                yield return p;
            }
        }

        private string GetBaseQuanDlUrl()
        {
            string url = $"{_quandlUrl}/datatables/WIKI/PRICES.csv" +
                $"?qopts.columns=ticker,date,close,adj_close,volume" +
                $"&api_key={Configuration.QuanDlApiKey}";

            return url;

        }

        #endregion
    }
}
