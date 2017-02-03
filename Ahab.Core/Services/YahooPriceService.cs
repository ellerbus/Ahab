using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Ahab.Core.Services
{
    class YahooPriceService
    {
        #region Members

        private IDownloaderService _downloader;

        private enum HistoryTypes
        {
            DividendHistory,
            Day,
            Week,
            Month,
        }

        private const string BaseUrl = "http://real-chart.finance.yahoo.com/table.csv?s=";

        private static readonly Dictionary<string, string> Exchanges = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            //Asia Pacific Stock Exchanges
            {"ASX", "AX"},
            {"HKG", "HK"},
            {"SHA", "SS"},
            {"SHE", "SZ"},
            {"NSE", "NS"},
            {"BSE", "BO"},
            {"JAK", "JK"},
            {"SEO", "KS"},
            {"KDQ", "KQ"},
            {"KUL", "KL"},
            {"NZE", "NZ"},
            {"SIN", "SI"},
            {"TPE", "TW"},
            //European Stock Exchanges
            {"WBAG", "VI"},
            {"EBR", "BR"},
            {"EPA", "PA"},
            {"BER", "BE"},
            {"ETR", "DE"},
            {"FRA", "F"},
            {"STU", "SG"},
            {"ISE", "IR"},
            {"BIT", "MI"},
            {"AMS", "AS"},
            {"OSL", "OL"},
            {"ELI", "LS"},
            {"MCE", "MA"},
            {"VTX", "VX"},
            {"LON", "L"},
            //Middle Eastern Stock Exchanges
            {"TLV", "TA"},
            //North American Stock Exchanges
            {"TSE", "TO"},
            {"CVE", "V"},
            {"AMEX", "AMEX"},
            {"NASDAQ", "NASDAQ"},
            {"NYSE", "NYSE"},
        };

        #endregion

        #region Constructor

        public YahooPriceService(IDownloaderService downloader)
        {
            _downloader = downloader;
        }

        #endregion

        #region Exchange

        //public static string GetExchangeAndStockId(string exchange, string stockId)
        //{
        //    string exchangeSuffix = GetExchangeSuffix(exchange);

        //    string stockCode = exchangeSuffix.IsNullOrEmpty() ? stockId : $"{stockId}.{exchangeSuffix}";

        //    return stockCode.ToUpperInvariant();
        //}

        //private string GetExchangeSuffix(string exchange)
        //{
        //    string suffix;

        //    if (Exchanges.TryGetValue(exchange, out suffix))
        //    {
        //        return suffix;
        //    }

        //    throw new Exception($"The \"{exchange.ToUpperInvariant()}\" exchange is not supported.");
        //}

        #endregion

        #region Prices

        public IEnumerable<Price> GetDailyHistoricalPrices(string stockId, DateTime start, DateTime end)
        {
            return GetHistoricalPrices(stockId, HistoryTypes.Day, start, end);
        }

        public IEnumerable<Price> GetWeeklyHistoricalPrices(string stockId, DateTime start, DateTime end)
        {
            return GetHistoricalPrices(stockId, HistoryTypes.Week, start, end);
        }

        public IEnumerable<Price> GetMonthlyHistoricalPriceData(string stockId, DateTime start, DateTime end)
        {
            return GetHistoricalPrices(stockId, HistoryTypes.Month, start, end);
        }

        private IEnumerable<Price> GetHistoricalPrices(string stockId, HistoryTypes historyType, DateTime start, DateTime end)
        {
            string contents = GetHistoricalData(stockId, historyType, start, end);

            StringReader sr = new StringReader(contents);

            CsvConfiguration cfg = new CsvConfiguration() { HasHeaderRecord = true };

            cfg.RegisterClassMap<YahooPriceCsvMap>();

            CsvReader reader = new CsvReader(sr, cfg);

            while (reader.Read())
            {
                //Date,Open,High,Low,Close,Volume,Adj Close
                Price price = reader.GetRecord<Price>();

                price.StockId = stockId;

                price.MakeAdjustments();

                yield return price;
            }
        }

        private string GetHistoricalData(string stockId, HistoryTypes historyType, DateTime start, DateTime end)
        {
            string dateRangeParameter = GetDateRangeParameter(start, end);

            string historyTypeParameter = GetHistoryTypeParameter(historyType);

            string options = $"{dateRangeParameter}{historyTypeParameter}";

            string url = $"{BaseUrl}{stockId}{options}";

            string data = _downloader.GetString(url, $"{stockId}.yprices");

            return data;
        }

        #endregion

        #region Parameter Methods

        private string GetDateRangeParameter(DateTime start, DateTime end)
        {
            // API uses zero-based month numbering

            string startParam = $"&a={start.Month - 1}&b={start.Day}&c={start.Year}";

            string endParam = $"&d={end.Month - 1}&e={end.Day}&f={end.Year}";

            return $"{startParam}{endParam}";
        }

        private string GetHistoryTypeParameter(HistoryTypes historyType)
        {
            string code = "";

            switch (historyType)
            {
                case HistoryTypes.DividendHistory:
                    code = "v";
                    break;

                case HistoryTypes.Day:
                    code = "d";
                    break;

                case HistoryTypes.Week:
                    code = "w";
                    break;

                case HistoryTypes.Month:
                    code = "m";
                    break;
            }

            return $"&g={code}";
        }

        #endregion
    }
}
