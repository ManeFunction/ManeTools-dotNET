using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class DoubleHistoryCacheTests
    {
        [Test]
        public void Empty_AverageIsZero() =>
            Assert.AreEqual(0d, new DoubleHistoryCache(3).GetAverage());

        [Test]
        public void Average_UsesFilledSlotsOnly()
        {
            DoubleHistoryCache cache = new(4);
            cache.Append(1d);
            cache.Append(3d);
            Assert.AreEqual(2d, cache.GetAverage());
        }

        [Test]
        public void Append_WrapsAndDropsOldest()
        {
            DoubleHistoryCache cache = new(3);
            cache.Append(1d);
            cache.Append(2d);
            cache.Append(3d);
            cache.Append(4d);
            Assert.AreEqual(3d, cache.GetAverage());
        }

        [Test]
        public void Clear_ResetsAverage()
        {
            DoubleHistoryCache cache = new(2);
            cache.Append(10d);
            cache.Clear();
            Assert.AreEqual(0d, cache.GetAverage());
        }

        [Test]
        public void Length_MustBePositive() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new DoubleHistoryCache(0));
    }
}
