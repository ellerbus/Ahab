using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pequod.Core;
using Pequod.Core.Models;

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
            string file = Path.Combine(Configuration.CacheDirectory, "MSFT.yprices");

            string contents = File.ReadAllText(file);

            var mockDownloader = new Mock<IDownloaderService>();

            mockDownloader
                .Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(contents));

            IDataService svc = new DataService(mockDownloader.Object);

            //  MSFT is always pulled/reset from TEST:resources
            return svc.GetEndOfDayPrices("MSFT", DateTime.MinValue, DateTime.MaxValue);
        }
    }
}
