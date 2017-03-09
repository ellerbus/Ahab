using System.Diagnostics;
using Pequod.Core.Indicators;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pequod.Tests.Indicators
{
    [TestClass]
    public class RelativeStrengthIndexTests : BaseIndicatorTests
    {
        [TestMethod]
        public void RelativeStrengthIndex_ShouldBe_Correct()
        {
            //  arrange
            var rsi = new RelativeStrengthIndex(Prices, 14);

            var idx = Prices.Count - 1;

            //  act
            for (int i = 0; i < Prices.Count; i++)
            {
                double v = rsi.GetValue(i);

                double g = rsi.GetAverageGain(i);

                double l = rsi.GetAverageLoss(i);

                Debug.WriteLine("I=[{0:00}] V=[{1,8:N2}] G=[{2,8:N2}] L=[{3,8:N2}]", i, v, g, l);
            }

            double value = rsi.GetValue(idx);

            double gain = rsi.GetAverageGain(idx);

            double loss = rsi.GetAverageLoss(idx);

            //  assert
            loss.Should().BeApproximately(0.39, 0.01);
            gain.Should().BeApproximately(0.24, 0.01);
            value.Should().BeApproximately(38.55, 0.01);
        }
    }
}
