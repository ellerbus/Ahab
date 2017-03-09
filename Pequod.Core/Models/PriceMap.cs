using CsvHelper.Configuration;

namespace Pequod.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    class PriceMap : CsvClassMap<Price>
    {
        public static CsvConfiguration GetConfigurationForQuanDL()
        {
            PriceMap map = new PriceMap();

            map.Map(x => x.Ticker).Name("ticker");
            map.Map(x => x.Date).Name("date");
            map.Map(x => x.Close).Name("close");
            map.Map(x => x.AdjustedClose).Name("adj_close");

            CsvConfiguration cfg = new CsvConfiguration()
            {
                HasHeaderRecord = true,
                IsHeaderCaseSensitive = false
            };

            cfg.RegisterClassMap(map);

            return cfg;
        }

        public static CsvConfiguration GetConfigurationForYahoo()
        {
            PriceMap map = new PriceMap();

            map.Map(x => x.Date).Name("date");
            map.Map(x => x.Open).Name("open");
            map.Map(x => x.High).Name("high");
            map.Map(x => x.Low).Name("low");
            map.Map(x => x.Close).Name("close");
            map.Map(x => x.Volume).Name("volume");
            map.Map(x => x.AdjustedClose).Name("adj close");

            CsvConfiguration cfg = new CsvConfiguration()
            {
                HasHeaderRecord = true,
                IsHeaderCaseSensitive = false
            };

            cfg.RegisterClassMap(map);

            return cfg;
        }
    }
}
