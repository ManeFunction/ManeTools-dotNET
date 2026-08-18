using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class ArrayExtensionsTests
    {
        [Test]
        public void FillWith_Value_FillsAndReturnsSameArray()
        {
            int[] array = { 0, 0, 0 };
            int[] result = array.FillWith(7);
            Assert.AreSame(array, result);
            CollectionAssert.AreEqual(new[] { 7, 7, 7 }, array);
        }

        [Test]
        public void FillWith_Factory_AndIndexedFactory()
        {
            CollectionAssert.AreEqual(new[] { 4, 4 }, new int[2].FillWith(() => 4));
            CollectionAssert.AreEqual(new[] { 0, 10, 20 }, new int[3].FillWith(i => i * 10));
        }

        [Test]
        public void FillWith_2D_ValueAndFactories()
        {
            int[,] values = new int[2, 2].FillWith(5);
            Assert.AreEqual(5, values[0, 0]);
            Assert.AreEqual(5, values[1, 1]);

            int n = 0;
            int[,] fromFactory = new int[2, 1].FillWith(() => ++n);
            Assert.AreEqual(1, fromFactory[0, 0]);
            Assert.AreEqual(2, fromFactory[1, 0]);

            int[,] indexed = new int[2, 2].FillWith((x, y) => x * 10 + y);
            Assert.AreEqual(0, indexed[0, 0]);
            Assert.AreEqual(1, indexed[0, 1]);
            Assert.AreEqual(10, indexed[1, 0]);
            Assert.AreEqual(11, indexed[1, 1]);
        }

        [Test]
        public void Clear_ZerosArray()
        {
            int[] array = { 1, 2, 3 };
            array.Clear();
            CollectionAssert.AreEqual(new[] { 0, 0, 0 }, array);

            int[,] grid = { { 1, 2 }, { 3, 4 } };
            grid.Clear();
            Assert.AreEqual(0, grid[0, 0]);
            Assert.AreEqual(0, grid[1, 1]);
        }

        [Test]
        public void FillWith_AndClear_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ((int[])null).FillWith(1));
            Assert.Throws<ArgumentNullException>(() => new int[1].FillWith((Func<int>)null));
            Assert.Throws<ArgumentNullException>(() => new int[1].FillWith((Func<int, int>)null));
            Assert.Throws<ArgumentNullException>(() => ((int[,])null).FillWith(1));
            Assert.Throws<ArgumentNullException>(() => new int[1, 1].FillWith((Func<int>)null));
            Assert.Throws<ArgumentNullException>(() => new int[1, 1].FillWith((Func<int, int, int>)null));
            Assert.Throws<ArgumentNullException>(() => ((int[])null).Clear());
            Assert.Throws<ArgumentNullException>(() => ((int[,])null).Clear());
        }
    }
}
