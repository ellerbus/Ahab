using System;
using System.Collections.Generic;
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
            Prices = new PriceCollection("x", GetPrices());
        }

        private IEnumerable<Price> GetPrices()
        {
            IDownloaderService d = new DownloaderService();

            IPriceService svc = new YahooPriceService(d);

            return svc.GetDailyHistoricalPrices("MSFT", DateTime.MinValue, DateTime.MaxValue);
        }
    }
}
