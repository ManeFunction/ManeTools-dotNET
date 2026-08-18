using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class DateTimeToolsTests
    {
        [Test]
        public void Max_ReturnsLater()
        {
            DateTime a = new(2020, 1, 1);
            DateTime b = new(2021, 1, 1);
            Assert.AreEqual(b, DateTimeTools.Max(a, b));
            Assert.AreEqual(b, DateTimeTools.Max(b, a));
            Assert.AreEqual(a, DateTimeTools.Max(a, a));
        }

        [Test]
        public void Min_ReturnsEarlier()
        {
            DateTime a = new(2020, 1, 1);
            DateTime b = new(2021, 1, 1);
            Assert.AreEqual(a, DateTimeTools.Min(a, b));
            Assert.AreEqual(a, DateTimeTools.Min(b, a));
        }
    }
}
