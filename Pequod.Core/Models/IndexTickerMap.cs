using CsvHelper.Configuration;

namespace Pequod.Core.Models
{
    class IndexTickerMap : CsvClassMap<IndexTicker>
    {
        public IndexTickerMap()
        {
            Map(x => x.Ticker).Name("ticker");
            Map(x => x.Name).Name("name");
            Map(x => x.FreeCode).Name("free_code");
            Map(x => x.PremiumCode).Name("premium_code");
        }

        public static CsvConfiguration GetConfiguration()
        {
            CsvConfiguration cfg = new CsvConfiguration()
            {
                HasHeaderRecord = true,
                IsHeaderCaseSensitive = false
            };

            cfg.RegisterClassMap<IndexTickerMap>();

            return cfg;
        }
    }
}
