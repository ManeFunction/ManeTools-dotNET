using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class MinMaxIntTests
    {
        [Test]
        public void MinMax_UnorderedEndpoints()
        {
            MinMaxInt range = new(5, 1);
            Assert.AreEqual(1, range.Min);
            Assert.AreEqual(5, range.Max);
            Assert.AreEqual(5, range.A);
            Assert.AreEqual(1, range.B);
        }

        [Test]
        public void Contains_InclusiveAndExclusive()
        {
            MinMaxInt range = new(1, 5);
            Assert.IsTrue(range.Contains(1));
            Assert.IsTrue(range.Contains(5));
            Assert.IsFalse(range.Contains(1, includeMin: false));
            Assert.IsFalse(range.Contains(5, includeMax: false));
            Assert.IsFalse(range.Contains(0));
            Assert.IsFalse(range.Contains(6));
        }

        [Test]
        public void Clamp_ToBounds()
        {
            MinMaxInt range = new(1, 5);
            Assert.AreEqual(1, range.Clamp(0));
            Assert.AreEqual(3, range.Clamp(3));
            Assert.AreEqual(5, range.Clamp(9));
        }

        [Test]
        public void Operators_AddSubtractScaleDivide()
        {
            MinMaxInt a = new(1, 5);
            MinMaxInt b = new(2, 3);
            Assert.AreEqual(new MinMaxInt(3, 8), a + b);
            Assert.AreEqual(new MinMaxInt(-2, 3), a - b);
            Assert.AreEqual(new MinMaxInt(2, 10), a * 2);
            Assert.AreEqual(new MinMaxInt(2, 10), 2 * a);
            Assert.AreEqual(new MinMaxInt(0, 2), a / 2);
        }

        [Test]
        public void Multiply_Overflow_Throws() =>
            Assert.Throws<OverflowException>(() => _ = new MinMaxInt(int.MaxValue, 1) * 2);

        [Test]
        public void Add_Overflow_Throws() =>
            Assert.Throws<OverflowException>(() => _ = new MinMaxInt(int.MaxValue, int.MaxValue) + new MinMaxInt(1, 1));

        [Test]
        public void Equality_NormalizesEndpoints()
        {
            Assert.IsTrue(new MinMaxInt(1, 5) == new MinMaxInt(5, 1));
            Assert.IsFalse(new MinMaxInt(1, 5) != new MinMaxInt(5, 1));
            Assert.IsTrue(new MinMaxInt(1, 5).Equals(new MinMaxInt(5, 1)));
            Assert.AreEqual(new MinMaxInt(1, 5).GetHashCode(), new MinMaxInt(5, 1).GetHashCode());
            Assert.IsFalse(new MinMaxInt(1, 5).Equals(new MinMaxInt(1, 6)));
            Assert.IsFalse(new MinMaxInt(1, 5).Equals(null));
        }

        [Test]
        public void ToString_NormalizedBounds() =>
            Assert.AreEqual("[1, 5]", new MinMaxInt(5, 1).ToString());
    }
}
