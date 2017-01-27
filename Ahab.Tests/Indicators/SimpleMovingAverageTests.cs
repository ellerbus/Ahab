using Ahab.Core.Indicators;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ahab.Tests.Indicators
{
    [TestClass]
    public class SimpleMovingAverageTests : BaseIndicatorTests
    {
        [TestMethod]
        public void SimpleMovingAverage_ShouldBe_Correct()
        {
            //  arrange
            var sma = new SimpleMovingAverage(Prices, 30);

            //  act
            double value = sma.GetValue(Prices.Count - 1);

            //  assert
            value.Should().BeApproximately(26.85, 0.01);
        }
    }
}
