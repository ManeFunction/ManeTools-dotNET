using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class PathToolsTests
    {
        [TestCase("a/b/c", "a/b")]
        [TestCase("a/b/c/", "a/b")]
        [TestCase("a\\b\\c", "a\\b")]
        [TestCase("a\\b\\c\\", "a\\b")]
        [TestCase("a/b\\c", "a/b")]
        [TestCase("/foo", "/")]
        [TestCase("/foo/", "/")]
        [TestCase("\\foo", "\\")]
        [TestCase("file", "")]
        [TestCase("", "")]
        [TestCase("/", "")]
        [TestCase("\\", "")]
        [TestCase("///", "")]
        public void RemoveLastComponent(string input, string expected) =>
            Assert.AreEqual(expected, PathTools.RemoveLastComponent(input));

        [Test]
        public void RemoveLastComponent_Null_Throws() =>
            Assert.Throws<ArgumentNullException>(() => PathTools.RemoveLastComponent(null));
    }
}
