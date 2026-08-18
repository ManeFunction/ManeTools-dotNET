using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class ListExtensionsTests
    {
        #region InitWith

        [Test]
        public void InitWith_ValueAndCount_ReplacesContents()
        {
            List<int> list = new() { 9 };
            list.InitWith(7, 3);
            CollectionAssert.AreEqual(new[] { 7, 7, 7 }, list);
        }

        [Test]
        public void InitWith_FactoryAndIndexedFactory()
        {
            List<int> a = new();
            a.InitWith(() => 4, 2);
            CollectionAssert.AreEqual(new[] { 4, 4 }, a);

            List<int> b = new();
            b.InitWith(i => i * 10, 3);
            CollectionAssert.AreEqual(new[] { 0, 10, 20 }, b);
        }

        [Test]
        public void InitWith_CapacityOverloads_UseExistingCapacity()
        {
            List<int> list = new(4);
            int capacity = list.Capacity;
            list.InitWith(1);
            Assert.AreEqual(capacity, list.Count);
            Assert.IsTrue(list.TrueForAll(v => v == 1));

            list = new List<int>(2);
            capacity = list.Capacity;
            list.InitWith(() => 5);
            Assert.AreEqual(capacity, list.Count);
            Assert.IsTrue(list.TrueForAll(v => v == 5));

            list = new List<int>(3);
            capacity = list.Capacity;
            list.InitWith(i => i);
            Assert.AreEqual(capacity, list.Count);
            for (int i = 0; i < capacity; i++)
                Assert.AreEqual(i, list[i]);
        }

        [Test]
        public void InitWith_CountZero_Clears()
        {
            List<int> list = new() { 1, 2 };
            list.InitWith(0, 0);
            CollectionAssert.IsEmpty(list);
        }

        [Test]
        public void InitWith_InvalidArgs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((List<int>)null).InitWith(1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new List<int>().InitWith(1, -1));
            Assert.Throws<ArgumentNullException>(() => new List<int>().InitWith((Func<int>)null, 1));
            Assert.Throws<ArgumentNullException>(() => new List<int>().InitWith((Func<int, int>)null, 1));
            Assert.Throws<ArgumentNullException>(() => ((List<int>)null).InitWith(1));
            Assert.Throws<ArgumentNullException>(() => ((List<int>)null).InitWith(() => 1));
            Assert.Throws<ArgumentNullException>(() => ((List<int>)null).InitWith(i => i));
        }

        #endregion

        #region Shuffle

        [Test]
        public void Shuffle_IsPermutation()
        {
            List<int> list = new() { 1, 2, 3, 4, 5 };
            list.Shuffle(new ManeRandom(42));
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, list);
        }

        [Test]
        public void Shuffle_EmptyAndSingle_NoThrow()
        {
            List<int> empty = new();
            empty.Shuffle(new ScriptedRandom());
            CollectionAssert.IsEmpty(empty);

            List<int> one = new() { 9 };
            one.Shuffle(new ScriptedRandom());
            CollectionAssert.AreEqual(new[] { 9 }, one);
        }

        [Test]
        public void Shuffle_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((List<int>)null).Shuffle(new ScriptedRandom()));
            Assert.Throws<ArgumentNullException>(() => new List<int> { 1 }.Shuffle(null));
        }

        #endregion

        #region RandomOrDefaultWithIdx

        [Test]
        public void RandomOrDefaultWithIdx_ReturnsElementAndIndex()
        {
            int value = new[] { 10, 20, 30 }.RandomOrDefaultWithIdx(out int idx, new ScriptedRandom(2));
            Assert.AreEqual(2, idx);
            Assert.AreEqual(30, value);
        }

        [Test]
        public void RandomOrDefaultWithIdx_Empty_ReturnsDefaultAndMinusOne()
        {
            int value = Array.Empty<int>().RandomOrDefaultWithIdx(out int idx, new ScriptedRandom());
            Assert.AreEqual(-1, idx);
            Assert.AreEqual(0, value);
        }

        [Test]
        public void RandomOrDefaultWithIdx_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ((IReadOnlyList<int>)null).RandomOrDefaultWithIdx(out _, new ScriptedRandom()));
            Assert.Throws<ArgumentNullException>(() =>
                new[] { 1 }.RandomOrDefaultWithIdx(out _, null));
        }

        #endregion
    }
}
