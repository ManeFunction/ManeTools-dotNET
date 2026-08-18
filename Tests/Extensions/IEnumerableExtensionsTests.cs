using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class IEnumerableExtensionsTests
    {
        #region ForEach

        [Test]
        public void ForEach_InvokesActionForEachElement()
        {
            List<int> seen = new();
            new[] { 1, 2, 3 }.ForEach(seen.Add);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, seen);
        }

        [Test]
        public void ForEach_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).ForEach(_ => { }));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.ForEach(null));
        }

        [Test]
        public void ForEachCancellable_StopsWhenFuncReturnsFalse()
        {
            List<int> seen = new();
            new[] { 1, 2, 3, 4 }.ForEachCancellable(v =>
            {
                seen.Add(v);
                return v < 2;
            });
            CollectionAssert.AreEqual(new[] { 1, 2 }, seen);
        }

        [Test]
        public void ForEachCancellable_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).ForEachCancellable(_ => true));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.ForEachCancellable(null));
        }

        #endregion

        #region GetRandomList

        [Test]
        public void GetRandomList_WithoutDuplicates_DoesNotExceedSource()
        {
            int[] source = { 1, 2, 3 };
            List<int> result = source.GetRandomList(10, new ManeRandom(1));
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEquivalent(source, result);
        }

        [Test]
        public void GetRandomList_WithDuplicates_ReachesRequestedCount()
        {
            int[] source = { 1, 2 };
            List<int> result = source.GetRandomList(5, new ScriptedRandom(), getMaxWithDuplicates: true);
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(v => v == 1 || v == 2));
        }

        [Test]
        public void GetRandomList_CountZeroOrEmptySource_ReturnsEmpty()
        {
            CollectionAssert.IsEmpty(new[] { 1 }.GetRandomList(0, new ScriptedRandom()));
            CollectionAssert.IsEmpty(Array.Empty<int>().GetRandomList(3, new ScriptedRandom()));
        }

        [Test]
        public void GetRandomList_InvalidArgs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).GetRandomList(1, new ScriptedRandom()));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.GetRandomList(1, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.GetRandomList(-1, new ScriptedRandom()));
        }

        #endregion

        #region RandomOrDefault

        [Test]
        public void RandomOrDefault_List_UsesIndex()
        {
            List<int> list = new() { 10, 20, 30 };
            Assert.AreEqual(20, list.RandomOrDefault(new ScriptedRandom(1)));
        }

        [Test]
        public void RandomOrDefault_HashSet_UsesCount()
        {
            HashSet<int> set = new() { 7 };
            Assert.AreEqual(7, set.RandomOrDefault(new ScriptedRandom()));
        }

        [Test]
        public void RandomOrDefault_Iterator_ReservoirSampling()
        {
            IEnumerable<int> Iter()
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }

            Assert.AreEqual(1, Iter().RandomOrDefault(new ScriptedRandom(0, 1, 1)));
        }

        [Test]
        public void RandomOrDefault_Empty_ReturnsDefault()
        {
            Assert.AreEqual(0, Array.Empty<int>().RandomOrDefault(new ScriptedRandom()));
            Assert.IsNull(new List<string>().RandomOrDefault(new ScriptedRandom()));
        }

        [Test]
        public void RandomOrDefault_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).RandomOrDefault(new ScriptedRandom()));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.RandomOrDefault(null));
        }

        #endregion

        #region SelfConcat

        [Test]
        public void SelfConcat_RepeatsSequence() =>
            CollectionAssert.AreEqual(new[] { 1, 2, 1, 2, 1, 2 }, new[] { 1, 2 }.SelfConcat(3).ToArray());

        [Test]
        public void SelfConcat_ZeroTimes_Empty() =>
            CollectionAssert.IsEmpty(new[] { 1 }.SelfConcat(0));

        [Test]
        public void SelfConcat_Once_SameInstance()
        {
            int[] source = { 1, 2 };
            Assert.AreSame(source, source.SelfConcat(1));
        }

        [Test]
        public void SelfConcat_InvalidArgs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).SelfConcat(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.SelfConcat(-1));
        }

        #endregion

        #region AnyAtLeast

        [Test]
        public void AnyAtLeast_Count_TrueWhenEnoughMatches()
        {
            int[] source = { 1, 2, 3, 4, 5 };
            Assert.IsTrue(source.AnyAtLeast(v => v % 2 == 0, 2));
            Assert.IsFalse(source.AnyAtLeast(v => v % 2 == 0, 3));
            Assert.IsTrue(source.AnyAtLeast(_ => true, 1));
        }

        [Test]
        public void AnyAtLeast_InvalidArgs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).AnyAtLeast(_ => true, 1));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.AnyAtLeast(null, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.AnyAtLeast(_ => true, 0));
        }

        #endregion

        #region IsNullOrEmpty

        [Test]
        public void IsNullOrEmpty_NullAndEmpty()
        {
            Assert.IsTrue(((IEnumerable<int>)null).IsNullOrEmpty());
            Assert.IsTrue(Array.Empty<int>().IsNullOrEmpty());
            Assert.IsTrue(new List<int>().IsNullOrEmpty());
            Assert.IsTrue("".IsNullOrEmpty());
            Assert.IsFalse(new[] { 1 }.IsNullOrEmpty());
            Assert.IsFalse("a".IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_PureEnumerable()
        {
            IEnumerable<int> Empty()
            {
                yield break;
            }

            IEnumerable<int> One()
            {
                yield return 1;
            }

            Assert.IsTrue(Empty().IsNullOrEmpty());
            Assert.IsFalse(One().IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_NonGenericCollection()
        {
            CountOnlyCollection empty = new() { Count = 0 };
            CountOnlyCollection filled = new() { Count = 2 };
            Assert.IsTrue(empty.IsNullOrEmpty());
            Assert.IsFalse(filled.IsNullOrEmpty());
        }

        private sealed class CountOnlyCollection : IEnumerable<int>, ICollection
        {
            public int Count { get; set; }
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public void CopyTo(Array array, int index) { }

            public IEnumerator<int> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion
    }
}
