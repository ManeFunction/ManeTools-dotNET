using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class GlobalRandomTests
    {
        [SetUp]
        public void SetUp() => GlobalRandom.ClearInstance();

        [TearDown]
        public void TearDown() => GlobalRandom.ClearInstance();

        [Test]
        public void Instance_CreatesManeRandom()
        {
            Assert.IsFalse(GlobalRandom.IsReady());

            IRandom random = GlobalRandom.Instance;

            Assert.IsTrue(GlobalRandom.IsReady());
            Assert.AreSame(random, GlobalRandom.Instance);
            int value = random.Next(0, 10);
            Assert.GreaterOrEqual(value, 0);
            Assert.Less(value, 10);
        }

        [Test]
        public void SetInstance_ReplacesSource()
        {
            GlobalRandom.SetInstance(new ScriptedRandom(4, 9));

            Assert.AreEqual(0, GlobalRandom.Instance.Seed);
            Assert.AreEqual(4, GlobalRandom.Instance.Next(0, 100));
            Assert.AreEqual(9, GlobalRandom.Instance.Next(0, 100));
            Assert.AreEqual(1, GlobalRandom.Instance.Next(1, 5));
            Assert.AreEqual(0f, GlobalRandom.Instance.Range01());
            Assert.AreEqual(0d, GlobalRandom.Instance.Range01Double());
        }

        [Test]
        public void SetInstance_Null_Throws() =>
            Assert.Throws<System.ArgumentNullException>(() => GlobalRandom.SetInstance(null));

        [Test]
        public void Instance_CanBePassedToDice()
        {
            GlobalRandom.SetInstance(new ScriptedRandom(3));

            int roll = RandomDice.Roll6(GlobalRandom.Instance);

            Assert.AreEqual(3, roll);
        }

        [Test]
        public void ClearInstance_DropsTheSource()
        {
            GlobalRandom.SetInstance(new ScriptedRandom(8));
            Assert.AreEqual(8, GlobalRandom.Instance.Next(0, 10));

            GlobalRandom.ClearInstance();

            Assert.IsFalse(GlobalRandom.IsReady());
        }
    }
}
