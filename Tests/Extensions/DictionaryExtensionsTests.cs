using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class DictionaryExtensionsTests
    {
        [Test]
        public void Convert_MapsKeysAndValues()
        {
            Dictionary<string, int> source = new()
            {
                ["a"] = 1,
                ["b"] = 2
            };

            Dictionary<string, string> result = source.Convert((k, v) => (k.ToUpperInvariant(), v.ToString()));
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1", result["A"]);
            Assert.AreEqual("2", result["B"]);
        }

        [Test]
        public void Convert_Empty_ReturnsEmpty()
        {
            Dictionary<int, int> result = new Dictionary<int, int>().Convert((k, v) => (k, v));
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Convert_DuplicateOutputKeys_Throws()
        {
            Dictionary<int, int> source = new() { [1] = 1, [2] = 2 };
            Assert.Throws<ArgumentException>(() => source.Convert((_, v) => (0, v)));
        }

        [Test]
        public void Convert_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ((IReadOnlyDictionary<int, int>)null).Convert((k, v) => (k, v)));
            Assert.Throws<ArgumentNullException>(() =>
                new Dictionary<int, int>().Convert<int, int, int, int>(null));
        }
    }
}
