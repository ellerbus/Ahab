using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pequod.Core.Models;

namespace Peqquod.Tests.Models
{
    [TestClass]
    public class PriceTests
    {
        [TestMethod]
        public void Price_Constructor_Should_Default()
        {
            var price = new Price();

            double.IsNaN(price.Close).Should().BeTrue();
            double.IsNaN(price.Open).Should().BeTrue();
            double.IsNaN(price.High).Should().BeTrue();
            double.IsNaN(price.Low).Should().BeTrue();

            price.AdjustedClose.Should().Be(0);
            price.Volume.Should().Be(0);
        }
    }
}
