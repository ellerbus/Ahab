using System;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Ahab.Core.Services
{
    class NasdaqSymbolCsvMap : CsvClassMap<Symbol>
    {
        class MarketCapTypeConverter : DoubleConverter
        {
            public override bool CanConvertTo(Type type)
            {
                if (type == typeof(MarketCaps))
                {
                    return true;
                }

                return base.CanConvertTo(type);
            }

            public override object ConvertFromString(TypeConverterOptions options, string text)
            {
                double d = (double)base.ConvertFromString(options, text);

                return d.ToMarketCapDescription();
            }
        }

        public NasdaqSymbolCsvMap()
        {
            //Date,Open,High,Low,Close,Volume,Adj Close
            Map(x => x.StockId).Name("SYMBOL", "Symbol");
            Map(x => x.Name).Name("NAME", "Name");
            Map(x => x.MarketCap).Name("MARKETCAP", "MarketCap").TypeConverter<MarketCapTypeConverter>();
            Map(x => x.Sector).Name("SECTOR", "Sector");
            Map(x => x.Industry).Name("INDUSTRY", "Industry");
        }
    }
}
