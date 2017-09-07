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

            map.Map(x => x.Symbol).Name("ticker");
            map.Map(x => x.Date).Name("date");
            map.Map(x => x.Close).Name("close");
            map.Map(x => x.AdjustedClose).Name("adj_close");
            map.Map(x => x.Volume).Name("volume");

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
