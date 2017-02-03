using System;
using System.Collections.Generic;
using System.IO;
using Ahab.Core;
using Ahab.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ahab.Tests.Indicators
{
    [TestClass]
    public class BaseIndicatorTests
    {
        protected PriceCollection Prices;

        [TestInitialize]
        public void Initialize()
        {
            string file = Path.Combine(Configuration.CacheDirectory, "MSFT.yprices");

            File.SetLastWriteTime(file, DateTime.Now);

            Prices = new PriceCollection("x", GetPrices());
        }

        private IEnumerable<Price> GetPrices()
        {
            IDownloaderService d = new DownloaderService();

            IAhabDataService svc = new AhabDataService(d);

            //  MSFT is always pulled/reset from TEST:resources
            return svc.GetDailyHistoricalPrices("MSFT", DateTime.MinValue, DateTime.MaxValue);
        }
    }
}
