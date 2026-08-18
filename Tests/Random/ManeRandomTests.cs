using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class ManeRandomTests
    {
        [Test]
        public void SeededConstructor_IsDeterministic()
        {
            ManeRandom a = new(123);
            ManeRandom b = new(123);
            Assert.AreEqual(123, a.Seed);
            Assert.AreEqual(a.Next(0, 1000), b.Next(0, 1000));
            Assert.AreEqual(a.Range01(), b.Range01());
            Assert.AreEqual(a.Range01Double(), b.Range01Double());
        }

        [Test]
        public void Next_IsWithinRange()
        {
            ManeRandom random = new(1);
            for (int i = 0; i < 50; i++)
            {
                int value = random.Next(3, 8);
                Assert.GreaterOrEqual(value, 3);
                Assert.Less(value, 8);
            }
        }

        [Test]
        public void Range01_IsInUnitInterval()
        {
            ManeRandom random = new(1);
            for (int i = 0; i < 50; i++)
            {
                float f = random.Range01();
                double d = random.Range01Double();
                Assert.GreaterOrEqual(f, 0f);
                Assert.Less(f, 1f);
                Assert.GreaterOrEqual(d, 0d);
                Assert.Less(d, 1d);
            }
        }

        [Test]
        public void ParameterlessConstructor_HasSeedAndWorks()
        {
            ManeRandom random = new();
            int value = random.Next(0, 10);
            Assert.GreaterOrEqual(value, 0);
            Assert.Less(value, 10);
        }

        [Test]
        public void Next_MinGreaterThanMax_Throws() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new ManeRandom(1).Next(5, 1));
    }
}
