using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class MinMaxFloatTests
    {
        [Test]
        public void MinMax_UnorderedEndpoints()
        {
            MinMaxFloat range = new(5f, 1f);
            Assert.AreEqual(1f, range.Min);
            Assert.AreEqual(5f, range.Max);
        }

        [Test]
        public void Contains_And_Clamp()
        {
            MinMaxFloat range = new(1f, 5f);
            Assert.IsTrue(range.Contains(1f));
            Assert.IsFalse(range.Contains(1f, includeMin: false));
            Assert.AreEqual(1f, range.Clamp(0f));
            Assert.AreEqual(5f, range.Clamp(9f));
        }

        [Test]
        public void Operators()
        {
            MinMaxFloat a = new(1f, 5f);
            MinMaxFloat b = new(2f, 3f);
            Assert.AreEqual(new MinMaxFloat(3f, 8f), a + b);
            Assert.AreEqual(new MinMaxFloat(-2f, 3f), a - b);
            Assert.AreEqual(new MinMaxFloat(2f, 10f), a * 2f);
            Assert.AreEqual(new MinMaxFloat(2f, 10f), 2f * a);
            Assert.AreEqual(new MinMaxFloat(.5f, 2.5f), a / 2f);
        }

        [Test]
        public void Equality_UsesToleranceAndNormalizes()
        {
            MinMaxFloat a = new(1f, 5f);
            MinMaxFloat flipped = new(5f, 1f);
            MinMaxFloat close = new(1f + 1e-7f, 5f);

            Assert.IsTrue(a == flipped);
            Assert.IsTrue(a == close);
            Assert.IsTrue(a != new MinMaxFloat(1f, 6f));
            Assert.AreEqual(a.GetHashCode(), flipped.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), close.GetHashCode());
            Assert.AreEqual("[1, 5]", flipped.ToString());
        }
    }
}
