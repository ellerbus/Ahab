using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Pequod.Core.Models;

namespace Pequod.Core
{
    public interface IDataService
    {
        IEnumerable<IndexTicker> GetComponentsOfSp500();

        IEnumerable<Price> GetEndOfDayPrices(DateTime date);

        IEnumerable<Price> GetEndOfDayPrices(string ticker, DateTime start, DateTime end);
    }

    public class DataService : IDataService
    {
        #region Members

        private const string _quandlUrl = "https://www.quandl.com/api/v3";

        private const string _yahooUrl = "http://real-chart.finance.yahoo.com/table.csv";

        private IDownloaderService _downloader;

        #endregion

        #region Constructors

        public DataService(IDownloaderService downloader)
        {
            _downloader = downloader;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IndexTicker> GetComponentsOfSp500()
        {
            string url = "https://s3.amazonaws.com/static.quandl.com/tickers/SP500.csv";

            string content = _downloader.GetStringAsync(url, "SP500.csv").Result;

            CsvConfiguration cfg = IndexTickerMap.GetConfiguration();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            return csvReader.GetRecords<IndexTicker>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<Price> GetEndOfDayPrices(DateTime date)
        {
            string url = $"{_quandlUrl}/datatables/WIKI/PRICES.csv" +
                $"?qopts.columns=ticker,date,close,adj_close" +
                $"&api_key={Configuration.QuanDlApiKey}" +
                $"&date={date:yyyy-MM-dd}";

            string content = _downloader.GetStringAsync(url, $"EOD-{date:yyyyMMdd}.csv").Result;

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
        public IEnumerable<Price> GetEndOfDayPrices(string ticker, DateTime start, DateTime end)
        {
            string url = $"{_yahooUrl}?s={ticker}&g=d" +                //daily
                $"&a={start.Month - 1}&b={start.Day}&c={start.Year}" +  //start
                $"&d={end.Month - 1}&e={end.Day}&f={end.Year}" +        //end
                $""
                ;

            string content = _downloader.GetStringAsync(url, $"EOD-{ticker}-{start:yyyyMMdd}-{end:yyyyMMdd}.csv").Result;

            CsvConfiguration cfg = PriceMap.GetConfigurationForYahoo();

            CsvReader csvReader = new CsvReader(new StringReader(content), cfg);

            foreach (Price p in csvReader.GetRecords<Price>())
            {
                p.Ticker = ticker;

                p.MakeAdjustments();

                yield return p;
            }
        }

        #endregion
    }
}
