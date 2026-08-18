using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class FloatHistoryCacheTests
    {
        [Test]
        public void Empty_AverageIsZero() =>
            Assert.AreEqual(0f, new FloatHistoryCache(3).GetAverage());

        [Test]
        public void Average_UsesFilledSlotsOnly()
        {
            FloatHistoryCache cache = new(4);
            cache.Append(1f);
            cache.Append(3f);
            Assert.AreEqual(2f, cache.GetAverage());
        }

        [Test]
        public void Append_WrapsAndDropsOldest()
        {
            FloatHistoryCache cache = new(3);
            cache.Append(1f);
            cache.Append(2f);
            cache.Append(3f);
            cache.Append(4f);
            Assert.AreEqual(3f, cache.GetAverage());
        }

        [Test]
        public void Clear_ResetsAverage()
        {
            FloatHistoryCache cache = new(2);
            cache.Append(10f);
            cache.Clear();
            Assert.AreEqual(0f, cache.GetAverage());
            cache.Append(4f);
            Assert.AreEqual(4f, cache.GetAverage());
        }

        [Test]
        public void Length_MustBePositive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new FloatHistoryCache(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new FloatHistoryCache(-1));
        }
    }
}
