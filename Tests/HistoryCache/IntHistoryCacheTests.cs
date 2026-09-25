using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class IntHistoryCacheTests
    {
        [Test]
        public void Empty_AverageIsZero() =>
            Assert.AreEqual(0, new IntHistoryCache(3).GetAverage());

        [Test]
        public void Average_UsesFilledSlotsOnly()
        {
            IntHistoryCache cache = new(4);
            cache.Append(1);
            cache.Append(3);
            Assert.AreEqual(2, cache.GetAverage());
        }

        [Test]
        public void Average_RoundsMidpointAwayFromZero()
        {
            IntHistoryCache cache = new(2);
            cache.Append(1);
            cache.Append(2);
            Assert.AreEqual(2, cache.GetAverage());

            cache.Clear();
            cache.Append(-1);
            cache.Append(-2);
            Assert.AreEqual(-2, cache.GetAverage());
        }

        [Test]
        public void Average_SumsInLong()
        {
            IntHistoryCache cache = new(2);
            cache.Append(int.MaxValue);
            cache.Append(int.MaxValue - 1);
            Assert.AreEqual(int.MaxValue, cache.GetAverage());

            cache.Clear();
            cache.Append(int.MinValue);
            cache.Append(int.MinValue + 1);
            Assert.AreEqual(int.MinValue, cache.GetAverage());
        }

        [Test]
        public void Append_WrapsAndDropsOldest()
        {
            IntHistoryCache cache = new(3);
            cache.Append(1);
            cache.Append(2);
            cache.Append(3);
            cache.Append(4);
            Assert.AreEqual(3, cache.GetAverage());
        }

        [Test]
        public void Clear_ResetsAverage()
        {
            IntHistoryCache cache = new(2);
            cache.Append(10);
            cache.Clear();
            Assert.AreEqual(0, cache.GetAverage());
            cache.Append(4);
            Assert.AreEqual(4, cache.GetAverage());
        }

        [Test]
        public void Length_MustBePositive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new IntHistoryCache(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new IntHistoryCache(-1));
        }
    }
}
