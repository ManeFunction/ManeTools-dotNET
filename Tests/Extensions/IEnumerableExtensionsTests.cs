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

        [Test]
        public void RandomOrDefault_Predicate_ArrayAndList_Reservoir()
        {
            Assert.AreEqual(3, new[] { 1, 2, 3 }.RandomOrDefault(v => v > 1, new ScriptedRandom(0)));
            Assert.AreEqual(1, new[] { 1, 2, 3 }.RandomOrDefault(_ => true, new ScriptedRandom(1, 1)));

            List<int> list = new() { 1, 2, 3 };
            Assert.AreEqual(2, list.RandomOrDefault(v => v > 1, new ScriptedRandom(1)));
            Assert.AreEqual(3, list.RandomOrDefault(_ => true, new ScriptedRandom(1, 0)));
        }

        [Test]
        public void RandomOrDefault_Predicate_OtherLists_UseIndexer()
        {
            IndexOnlyReadOnlyList readOnlyList = new(1, 2, 3);
            Assert.AreEqual(2, readOnlyList.RandomOrDefault(v => v > 1, new ScriptedRandom(1)));
            Assert.AreEqual(0, readOnlyList.Enumerations);

            IndexOnlyList list = new(1, 2, 3);
            Assert.AreEqual(3, list.RandomOrDefault(v => v > 1, new ScriptedRandom(0)));
            Assert.AreEqual(0, list.Enumerations);
        }

        [Test]
        public void RandomOrDefault_Predicate_Iterator_Reservoir()
        {
            IEnumerable<int> Iter()
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }

            Assert.AreEqual(3, Iter().RandomOrDefault(v => v > 1, new ScriptedRandom(0)));
            Assert.AreEqual(2, Iter().RandomOrDefault(v => v == 2, new NoDrawRandom()));
        }

        [Test]
        public void RandomOrDefault_Predicate_EmptyCountable_DoesNotEnumerate()
        {
            EmptyReadOnlyCollection readOnly = new();
            Assert.AreEqual(0, readOnly.RandomOrDefault(_ => true, new NoDrawRandom()));
            Assert.AreEqual(0, readOnly.Enumerations);

            EmptyBag bag = new();
            Assert.AreEqual(0, bag.RandomOrDefault(_ => true, new NoDrawRandom()));
            Assert.AreEqual(0, bag.Enumerations);
        }

        [Test]
        public void RandomOrDefault_Predicate_NoChoice_DoesNotDraw()
        {
            Assert.AreEqual(0, Array.Empty<int>().RandomOrDefault(_ => true, new NoDrawRandom()));
            Assert.AreEqual(0, new List<int> { 1, 2 }.RandomOrDefault(v => v < 0, new NoDrawRandom()));
            Assert.AreEqual(0, new HashSet<int>().RandomOrDefault(_ => true, new NoDrawRandom()));
            Assert.AreEqual(2, new HashSet<int> { 1, 2, 3 }.RandomOrDefault(v => v == 2, new NoDrawRandom()));
            Assert.AreEqual(2, new[] { 1, 2, 3 }.RandomOrDefault(v => v == 2, new NoDrawRandom()));
            Assert.IsNull(new List<string> { "a" }.RandomOrDefault(s => s.Length > 1, new NoDrawRandom()));
        }

        [Test]
        public void RandomOrDefault_Predicate_RunsOncePerElement()
        {
            int calls = 0;
            int picked = new List<int> { 1, 2, 3, 4 }.RandomOrDefault(v =>
            {
                calls++;
                return v % 2 == 0;
            }, new ScriptedRandom(0));

            Assert.AreEqual(4, calls);
            Assert.AreEqual(4, picked);
        }

        [Test]
        public void RandomOrDefault_Predicate_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ((IEnumerable<int>)null).RandomOrDefault(_ => true, new ScriptedRandom()));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.RandomOrDefault((Func<int, bool>)null, new ScriptedRandom()));
            Assert.Throws<ArgumentNullException>(() => new[] { 1 }.RandomOrDefault(_ => true, null));
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

        private sealed class NoDrawRandom : IRandom
        {
            public int Seed => 0;
            public int Next(int min, int max) => throw new InvalidOperationException();
            public double Range01Double() => throw new InvalidOperationException();
            public float Range01() => throw new InvalidOperationException();
        }

        private sealed class IndexOnlyReadOnlyList : IReadOnlyList<int>
        {
            private readonly int[] _items;
            public int Enumerations { get; private set; }

            public IndexOnlyReadOnlyList(params int[] items) => _items = items;

            public int Count => _items.Length;
            public int this[int index] => _items[index];

            public IEnumerator<int> GetEnumerator()
            {
                Enumerations++;
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private sealed class IndexOnlyList : IList<int>
        {
            private readonly int[] _items;
            public int Enumerations { get; private set; }

            public IndexOnlyList(params int[] items) => _items = items;

            public int this[int index]
            {
                get => _items[index];
                set => throw new NotSupportedException();
            }

            public int Count => _items.Length;
            public bool IsReadOnly => true;

            public IEnumerator<int> GetEnumerator()
            {
                Enumerations++;
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public void Add(int item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(int item) => throw new NotSupportedException();
            public void CopyTo(int[] array, int arrayIndex) { }
            public int IndexOf(int item) => throw new NotSupportedException();
            public void Insert(int index, int item) => throw new NotSupportedException();
            public bool Remove(int item) => throw new NotSupportedException();
            public void RemoveAt(int index) => throw new NotSupportedException();
        }

        private sealed class EmptyReadOnlyCollection : IReadOnlyCollection<int>
        {
            public int Enumerations { get; private set; }
            public int Count => 0;

            public IEnumerator<int> GetEnumerator()
            {
                Enumerations++;
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private sealed class EmptyBag : ICollection<int>
        {
            public int Enumerations { get; private set; }
            public int Count => 0;
            public bool IsReadOnly => true;

            public IEnumerator<int> GetEnumerator()
            {
                Enumerations++;
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public void Add(int item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(int item) => false;
            public void CopyTo(int[] array, int arrayIndex) { }
            public bool Remove(int item) => throw new NotSupportedException();
        }
    }
}
