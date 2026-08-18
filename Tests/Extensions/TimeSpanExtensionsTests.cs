using System;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class TimeSpanExtensionsTests
    {
        [Test]
        public void ToHHMMSS_FormatsFullHours()
        {
            Assert.AreEqual("00:00:05", TimeSpan.FromSeconds(5).ToHHMMSS());
            Assert.AreEqual("01:02:03", new TimeSpan(1, 2, 3).ToHHMMSS());
            Assert.AreEqual("25:00:00", TimeSpan.FromHours(25).ToHHMMSS());
        }

        [Test]
        public void ToHHMM_FormatsHoursAndMinutes()
        {
            Assert.AreEqual("01:02", new TimeSpan(1, 2, 9).ToHHMM());
            Assert.AreEqual("25:00", TimeSpan.FromHours(25).ToHHMM());
        }

        [Test]
        public void ToMMSS_FormatsTotalMinutes()
        {
            Assert.AreEqual("00:05", TimeSpan.FromSeconds(5).ToMMSS());
            Assert.AreEqual("90:00", TimeSpan.FromMinutes(90).ToMMSS());
        }

        [Test]
        public void NegativeAndZero_FormatAsZeros()
        {
            Assert.AreEqual("00:00:00", TimeSpan.Zero.ToHHMMSS());
            Assert.AreEqual("00:00:00", TimeSpan.FromSeconds(-1).ToHHMMSS());
            Assert.AreEqual("00:00", TimeSpan.Zero.ToHHMM());
            Assert.AreEqual("00:00", TimeSpan.FromSeconds(-1).ToHHMM());
            Assert.AreEqual("00:00", TimeSpan.Zero.ToMMSS());
            Assert.AreEqual("00:00", TimeSpan.FromSeconds(-1).ToMMSS());
        }
    }
}
