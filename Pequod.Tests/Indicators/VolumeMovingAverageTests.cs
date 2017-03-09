using Pequod.Core.Indicators;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pequod.Tests.Indicators
{
    [TestClass]
    public class VolumeMovingAverageTests : BaseIndicatorTests
    {
        [TestMethod]
        public void VolumeMovingAverage_ShouldBe_Correct()
        {
            //  arrange
            var vma = new VolumeMovingAverage(Prices, 10);

            //  act
            double value = vma.GetValue(Prices.Count - 1);

            //  assert
            value.Should().BeApproximately(89316520, 1);
        }
    }
}
