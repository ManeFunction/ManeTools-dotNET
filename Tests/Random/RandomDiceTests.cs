using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class RandomDiceTests
    {
        [Test]
        public void Roll_UsesInclusiveOneToSides()
        {
            Assert.AreEqual(4, RandomDice.Roll(6, new ScriptedRandom(4)));
            Assert.AreEqual(1, RandomDice.Roll(1, new ScriptedRandom(1)));
        }

        [Test]
        public void ConvenienceRolls_StayInRange()
        {
            ManeRandom random = new(7);
            AssertInRange(RandomDice.Roll4(random), 1, 4);
            AssertInRange(RandomDice.Roll6(random), 1, 6);
            AssertInRange(RandomDice.Roll8(random), 1, 8);
            AssertInRange(RandomDice.Roll10(random), 1, 10);
            AssertInRange(RandomDice.Roll12(random), 1, 12);
            AssertInRange(RandomDice.Roll20(random), 1, 20);
        }

        [Test]
        public void Roll_InvalidArgs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => RandomDice.Roll(6, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomDice.Roll(0, new ScriptedRandom()));
        }

        private static void AssertInRange(int value, int min, int max)
        {
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }
    }
}
