using Ahab.Core.Indicators;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ahab.Tests.Indicators
{
    [TestClass]
    public class ExponentialMovingAverageTests : BaseIndicatorTests
    {
        [TestMethod]
        public void ExponentialMovingAverage_ShouldBe_Correct()
        {
            //  arrange
            var ema = new ExponentialMovingAverage(Prices, 30);

            //  act
            double value = ema.GetValue(Prices.Count - 1);

            //  assert
            value.Should().BeApproximately(26.72, 0.01);
        }
    }
}
