using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pequod.Core.Models;

namespace Peqquod.Tests.Models
{
    [TestClass]
    public class SplitTests
    {
        [TestMethod]
        public void Split_Ratio_Should_CalculateAdjustment()
        {
            var actual = new Split() { Ratio = "2-1" };

            actual.Ratio.Should().Be("2:1");
            actual.Adjustment.Should().Be(0.5);

            actual = new Split() { Ratio = "10000:4793" };

            actual.Ratio.Should().Be("2:1");
            actual.Adjustment.Should().Be(0.5);
        }
    }
}
