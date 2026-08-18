using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class NumericExtensionsTests
    {
        #region Clamp

        [Test]
        public void Clamp_InsideRange_ReturnsValue() =>
            Assert.AreEqual(5, 5.Clamp(0, 10));

        [Test]
        public void Clamp_BelowMin_ReturnsMin() =>
            Assert.AreEqual(0, (-1).Clamp(0, 10));

        [Test]
        public void Clamp_AboveMax_ReturnsMax() =>
            Assert.AreEqual(10, 11.Clamp(0, 10));

        [Test]
        public void Clamp_MinEqualsMax_ReturnsBound() =>
            Assert.AreEqual(3, 7.Clamp(3, 3));

        [Test]
        public void Clamp_MinGreaterThanMax_Throws() =>
            Assert.Throws<ArgumentException>(() => 1.Clamp(10, 0));

        [Test]
        public void ClampMin_And_ClampMax()
        {
            Assert.AreEqual(3, 1.ClampMin(3));
            Assert.AreEqual(5, 5.ClampMin(3));
            Assert.AreEqual(3, 5.ClampMax(3));
            Assert.AreEqual(1, 1.ClampMax(3));
        }

        [Test]
        public void Clamp01_FloatDoubleDecimal()
        {
            Assert.AreEqual(0f, (-1f).Clamp01());
            Assert.AreEqual(1f, 2f.Clamp01());
            Assert.AreEqual(.5f, .5f.Clamp01());
            Assert.AreEqual(0d, (-.1d).Clamp01());
            Assert.AreEqual(1d, 1.1d.Clamp01());
            Assert.AreEqual(0m, (-1m).Clamp01());
            Assert.AreEqual(1m, 2m.Clamp01());
        }

        #endregion

        #region Parity

        [TestCase(0, true)]
        [TestCase(2, true)]
        [TestCase(-2, true)]
        [TestCase(1, false)]
        [TestCase(-1, false)]
        [TestCase(3, false)]
        public void IsEven_And_IsOdd(int value, bool even)
        {
            Assert.AreEqual(even, value.IsEven());
            Assert.AreEqual(!even, value.IsOdd());
        }

        #endregion

        #region Remap

        [Test]
        public void Remap_Linear()
        {
            Assert.AreEqual(5f, .5f.Remap(0f, 1f, 0f, 10f));
            Assert.AreEqual(5d, .5d.Remap(0d, 1d, 0d, 10d));
            Assert.AreEqual(-1f, 0f.Remap(0f, 1f, -1f, 1f));
            Assert.AreEqual(1f, 1f.Remap(0f, 1f, -1f, 1f));
        }

        [Test]
        public void Remap_ZeroWidthSource_ReturnsDestFrom()
        {
            Assert.AreEqual(10f, 99f.Remap(5f, 5f, 10f, 20f));
            Assert.AreEqual(10d, 99d.Remap(5d, 5d, 10d, 20d));
        }

        #endregion

        #region Safe arithmetic

        [Test]
        public void SafePlus_NoOverflow() =>
            Assert.AreEqual(5, 2.SafePlus(3));

        [Test]
        public void SafePlus_Overflow_Saturates()
        {
            Assert.AreEqual(int.MaxValue, int.MaxValue.SafePlus(1));
            Assert.AreEqual(int.MinValue, int.MinValue.SafePlus(-1));
            Assert.AreEqual(int.MaxValue, 1.SafePlus(int.MaxValue));
        }

        [Test]
        public void SafeMinus_NoOverflow() =>
            Assert.AreEqual(1, 4.SafeMinus(3));

        [Test]
        public void SafeMinus_Overflow_Saturates()
        {
            Assert.AreEqual(int.MinValue, int.MinValue.SafeMinus(1));
            Assert.AreEqual(int.MaxValue, int.MaxValue.SafeMinus(-1));
        }

        [Test]
        public void SafeMultiply_NoOverflow() =>
            Assert.AreEqual(12, 3.SafeMultiply(4));

        [Test]
        public void SafeMultiply_Overflow_SaturatesBySign()
        {
            Assert.AreEqual(int.MaxValue, int.MaxValue.SafeMultiply(2));
            Assert.AreEqual(int.MinValue, int.MaxValue.SafeMultiply(-2));
            Assert.AreEqual(int.MinValue, int.MinValue.SafeMultiply(2));
            Assert.AreEqual(int.MaxValue, int.MinValue.SafeMultiply(-1));
        }

        #endregion
    }
}
