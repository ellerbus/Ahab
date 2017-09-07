using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pequod.Core.Models;
using Pequod.Tests.Properties;

namespace Pequod.Tests.Indicators
{
    [TestClass]
    public class BaseIndicatorTests
    {
        protected PriceCollection Prices;

        [TestInitialize]
        public void Initialize()
        {
            Prices = new PriceCollection("x", GetPrices());
        }

        private IEnumerable<Price> GetPrices()
        {
            CsvConfiguration cfg = PriceMap.ConfigurationForYahoo();

            CsvReader csvReader = new CsvReader(new StringReader(Resources.MSFT), cfg);

            foreach (Price p in csvReader.GetRecords<Price>())
            {
                p.MakeAdjustments();

                yield return p;
            }
        }

        class PriceMap : CsvClassMap<Price>
        {
            public static CsvConfiguration ConfigurationForYahoo()
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
}