using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class MinMaxExtensionsTests
    {
        [Test]
        public void IsInRange_DelegatesToContains()
        {
            Assert.IsTrue(3.IsInRange(new MinMaxInt(1, 5)));
            Assert.IsFalse(1.IsInRange(new MinMaxInt(1, 5), includeMin: false));
            Assert.IsTrue(3f.IsInRange(new MinMaxFloat(1f, 5f)));
            Assert.IsTrue(3d.IsInRange(new MinMaxDouble(1d, 5d)));
        }

        [Test]
        public void Clamp_DelegatesToRange()
        {
            Assert.AreEqual(1, 0.Clamp(new MinMaxInt(1, 5)));
            Assert.AreEqual(5f, 9f.Clamp(new MinMaxFloat(1f, 5f)));
            Assert.AreEqual(3d, 3d.Clamp(new MinMaxDouble(1d, 5d)));
        }
    }
}
