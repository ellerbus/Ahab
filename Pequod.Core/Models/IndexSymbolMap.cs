using CsvHelper.Configuration;

namespace Pequod.Core.Models
{
    class IndexSymbolMap : CsvClassMap<IndexSymbol>
    {
        public static CsvConfiguration GetConfiguration()
        {
            CsvConfiguration cfg = new CsvConfiguration()
            {
                HasHeaderRecord = true,
                IsHeaderCaseSensitive = false
            };

            IndexSymbolMap map = new IndexSymbolMap();

            map.Map(x => x.Symbol).Name("symbol");
            map.Map(x => x.Name).Name("name");
            map.Map(x => x.Sector).Name("sector");

            cfg.RegisterClassMap(map);

            return cfg;
        }
    }
}
