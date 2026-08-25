using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class RandomCoinTests
    {
        [Test]
        public void Flip_UsesTwoSidedDie()
        {
            Assert.IsTrue(RandomCoin.Flip(new ScriptedRandom(1)));
            Assert.IsFalse(RandomCoin.Flip(new ScriptedRandom(2)));
        }

        [Test]
        public void Roll_InvalidArgs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => RandomCoin.Flip(null));
        }
    }
}