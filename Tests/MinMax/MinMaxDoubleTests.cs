using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class MinMaxDoubleTests
    {
        [Test]
        public void MinMax_UnorderedEndpoints()
        {
            MinMaxDouble range = new(5d, 1d);
            Assert.AreEqual(1d, range.Min);
            Assert.AreEqual(5d, range.Max);
        }

        [Test]
        public void Contains_And_Clamp()
        {
            MinMaxDouble range = new(1d, 5d);
            Assert.IsTrue(range.Contains(5d));
            Assert.IsFalse(range.Contains(5d, includeMax: false));
            Assert.AreEqual(1d, range.Clamp(0d));
            Assert.AreEqual(5d, range.Clamp(9d));
        }

        [Test]
        public void Operators()
        {
            MinMaxDouble a = new(1d, 5d);
            MinMaxDouble b = new(2d, 3d);
            Assert.AreEqual(new MinMaxDouble(3d, 8d), a + b);
            Assert.AreEqual(new MinMaxDouble(-2d, 3d), a - b);
            Assert.AreEqual(new MinMaxDouble(2d, 10d), a * 2d);
            Assert.AreEqual(new MinMaxDouble(2d, 10d), 2d * a);
            Assert.AreEqual(new MinMaxDouble(.5d, 2.5d), a / 2d);
        }

        [Test]
        public void Equality_UsesToleranceAndNormalizes()
        {
            MinMaxDouble a = new(1d, 5d);
            MinMaxDouble flipped = new(5d, 1d);
            MinMaxDouble close = new(1d + 1e-13d, 5d);

            Assert.IsTrue(a == flipped);
            Assert.IsTrue(a == close);
            Assert.IsTrue(a != new MinMaxDouble(1d, 6d));
            Assert.AreEqual(a.GetHashCode(), flipped.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), close.GetHashCode());
            Assert.AreEqual("[1, 5]", flipped.ToString());
        }
    }
}
