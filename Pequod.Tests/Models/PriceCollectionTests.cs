using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pequod.Core.Models;

namespace Peqquod.Tests.Models
{
    [TestClass]
    public class PriceCollectionTests
    {
        [TestMethod]
        public void PriceCollection_MinDate_Should_Return_MaxValue()
        {
            var prices = new PriceCollection("X");

            prices.MinDate.Should().Be(DateTime.MaxValue);
        }

        [TestMethod]
        public void PriceCollection_MinDate_Should_Return_FirstDate()
        {
            var prices = new PriceCollection("X")
            {
                new Price() { Date = new DateTime(2000,1,1) },
                new Price() { Date = new DateTime(2000,12,31) }
            };

            prices.MinDate.Should().Be(new DateTime(2000, 1, 1));
        }


        [TestMethod]
        public void PriceCollection_MaxDate_Should_Return_MinValue()
        {
            var prices = new PriceCollection("X");

            prices.MaxDate.Should().Be(DateTime.MinValue);
        }


        [TestMethod]
        public void PriceCollection_MaxDate_Should_Return_LastDate()
        {
            var prices = new PriceCollection("X")
            {
                new Price() { Date = new DateTime(2000,1,1) },
                new Price() { Date = new DateTime(2000,12,31) }
            };

            prices.MaxDate.Should().Be(new DateTime(2000, 12, 31));
        }

        [TestMethod]
        public void PriceCollection_FindOrCreate_Should_AddOrdered()
        {
            var prices = new PriceCollection("X");

            var a = prices.FindOrCreate(DateTime.Now.AddDays(1));
            var b = prices.FindOrCreate(DateTime.Now);

            prices.Count.Should().Be(2);

            prices[1].Should().BeSameAs(a);
            prices[0].Should().BeSameAs(b);
        }

        [TestMethod]
        public void PriceCollection_GetWindow_Should_ReturnCorrectWindow()
        {
            var prices = new PriceCollection("X", GetPrices());

            var window = prices.GetWindow(2, 3).ToList();

            window[0].Should().BeSameAs(prices[0]);
            window[1].Should().BeSameAs(prices[1]);
            window[2].Should().BeSameAs(prices[2]);
        }

        [TestMethod]
        public void PriceCollection_ShouldNot_AddSplits()
        {
            //  arrange
            var prices = new PriceCollection("X", GetPrices());

            var a = new Split() { Ratio = "1:2", Date = prices[0].Date.AddDays(-1) };

            //  act
            prices.AddSplit(a);

            //  assert (basically saying none should be set)
            prices
                .Count(x => x.AdjustmentMultiplier > 0)
                .Should().Be(0);
        }

        [TestMethod]
        public void PriceCollection_Should_AddSplit()
        {
            //  arrange
            var prices = new PriceCollection("X", GetPrices());

            var a = new Split() { Ratio = "1:2", Date = prices[1].Date };

            //  act
            prices.AddSplit(a);

            //  assert
            prices[0].AdjustmentMultiplier.Should().Be(a.Adjustment);

            //  assert (basically saying first one should be set)
            prices
                .Count(x => x.Date < a.Date && x.AdjustmentMultiplier > 0)
                .Should().Be(1);
        }

        [TestMethod]
        public void PriceCollection_Should_AddSplitCascade()
        {
            //  arrange
            var prices = new PriceCollection("X", GetPrices());

            var a = new Split() { Ratio = "1:2", Date = prices[1].Date };
            var b = new Split() { Ratio = "1:2", Date = prices[2].Date };

            //  act
            prices.AddSplit(a);
            prices.AddSplit(b);

            //  assert
            prices[0].AdjustmentMultiplier.Should().Be(a.Adjustment * b.Adjustment);
            prices[1].AdjustmentMultiplier.Should().Be(b.Adjustment);

            //  assert (basically saying first two should be set)
            prices
                .Count(x => x.Date < b.Date && x.AdjustmentMultiplier > 0)
                .Should().Be(2);
        }

        private IEnumerable<Price> GetPrices()
        {
            for (int idx = 0; idx < 10; idx++)
            {
                int value = idx + 1;

                var price = new Price()
                {
                    Date = DateTime.UtcNow.AddDays(value),
                    Volume = value * 100000,
                    Open = value,
                    Close = value,
                    Low = value,
                    High = value
                };

                yield return price;
            }
        }

    }
}
