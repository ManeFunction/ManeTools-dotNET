using System;
using System.Globalization;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class StringExtensionsTests
    {
        private CultureInfo _previousCulture;

        [SetUp]
        public void SetCommaDecimalCulture()
        {
            _previousCulture = CultureInfo.CurrentCulture;
            CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ",";
            culture.NumberFormat.NumberGroupSeparator = ".";
            CultureInfo.CurrentCulture = culture;
        }

        [TearDown]
        public void RestoreCulture() =>
            CultureInfo.CurrentCulture = _previousCulture;

        #region Parse integers — separators and spaces

        [TestCase("12")]
        [TestCase(" 12 ")]
        [TestCase("\t12\n")]
        [TestCase("1 2")]
        [TestCase("1'2")]
        [TestCase("1\u00A02")]
        public void ParseIntegers_SpacesAndApostrophe(string input)
        {
            Assert.AreEqual((sbyte)12, input.ParseSByteInvariant());
            Assert.AreEqual((byte)12, input.ParseByteInvariant());
            Assert.AreEqual((short)12, input.ParseShortInvariant());
            Assert.AreEqual((ushort)12, input.ParseUShortInvariant());
            Assert.AreEqual(12, input.ParseIntInvariant());
            Assert.AreEqual(12u, input.ParseUIntInvariant());
            Assert.AreEqual(12L, input.ParseLongInvariant());
            Assert.AreEqual(12ul, input.ParseULongInvariant());
        }

        [TestCase("1.234.567", 1234567)]
        [TestCase("1,234,567", 1234567)]
        [TestCase("1 234", 1234)]
        [TestCase("1'234", 1234)]
        [TestCase("1.000", 1)]
        [TestCase("1,000", 1)]
        public void ParseInt_GroupingAndDecimalZeros(string input, int expected)
        {
            Assert.AreEqual(expected, input.ParseIntInvariant());
            Assert.AreEqual((long)expected, input.ParseLongInvariant());
        }

        [TestCase("1.5")]
        [TestCase("1,5")]
        [TestCase("abc")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        [TestCase(".")]
        [TestCase(",")]
        public void ParseIntegers_Invalid_ReturnsDefault(string input)
        {
            Assert.AreEqual(0, input.ParseIntInvariant());
            Assert.AreEqual(42, input.ParseIntInvariant(42));
            Assert.AreEqual((sbyte)0, input.ParseSByteInvariant());
            Assert.AreEqual((byte)0, input.ParseByteInvariant());
            Assert.AreEqual((short)0, input.ParseShortInvariant());
            Assert.AreEqual((ushort)0, input.ParseUShortInvariant());
            Assert.AreEqual(0u, input.ParseUIntInvariant());
            Assert.AreEqual(0L, input.ParseLongInvariant());
            Assert.AreEqual(0ul, input.ParseULongInvariant());
        }

        #endregion

        #region Parse integers — type edges

        [Test]
        public void ParseSByte_Edges()
        {
            Assert.AreEqual(sbyte.MinValue, "-128".ParseSByteInvariant());
            Assert.AreEqual(sbyte.MaxValue, "127".ParseSByteInvariant());
            Assert.AreEqual(0, "128".ParseSByteInvariant());
            Assert.AreEqual(0, "-129".ParseSByteInvariant());
            Assert.AreEqual((sbyte)7, "128".ParseSByteInvariant(7));
        }

        [Test]
        public void ParseByte_Edges()
        {
            Assert.AreEqual(byte.MinValue, "0".ParseByteInvariant());
            Assert.AreEqual(byte.MaxValue, "255".ParseByteInvariant());
            Assert.AreEqual(0, "256".ParseByteInvariant());
            Assert.AreEqual(0, "-1".ParseByteInvariant());
            Assert.AreEqual((byte)7, "256".ParseByteInvariant(7));
        }

        [Test]
        public void ParseShort_Edges()
        {
            Assert.AreEqual(short.MinValue, "-32768".ParseShortInvariant());
            Assert.AreEqual(short.MaxValue, "32767".ParseShortInvariant());
            Assert.AreEqual(0, "32768".ParseShortInvariant());
            Assert.AreEqual(0, "-32769".ParseShortInvariant());
        }

        [Test]
        public void ParseUShort_Edges()
        {
            Assert.AreEqual(ushort.MinValue, "0".ParseUShortInvariant());
            Assert.AreEqual(ushort.MaxValue, "65535".ParseUShortInvariant());
            Assert.AreEqual(0, "65536".ParseUShortInvariant());
            Assert.AreEqual(0, "-1".ParseUShortInvariant());
        }

        [Test]
        public void ParseInt_Edges()
        {
            Assert.AreEqual(int.MinValue, int.MinValue.ToString(CultureInfo.InvariantCulture).ParseIntInvariant());
            Assert.AreEqual(int.MaxValue, int.MaxValue.ToString(CultureInfo.InvariantCulture).ParseIntInvariant());
            Assert.AreEqual(0, "2147483648".ParseIntInvariant());
            Assert.AreEqual(0, "-2147483649".ParseIntInvariant());
            Assert.AreEqual(-3, "-3".ParseIntInvariant());
        }

        [Test]
        public void ParseUInt_Edges()
        {
            Assert.AreEqual(uint.MinValue, "0".ParseUIntInvariant());
            Assert.AreEqual(uint.MaxValue, uint.MaxValue.ToString(CultureInfo.InvariantCulture).ParseUIntInvariant());
            Assert.AreEqual(0u, "4294967296".ParseUIntInvariant());
            Assert.AreEqual(0u, "-1".ParseUIntInvariant());
            Assert.AreEqual(3000000000u, "3 000 000 000".ParseUIntInvariant());
        }

        [Test]
        public void ParseLong_Edges()
        {
            Assert.AreEqual(long.MinValue, long.MinValue.ToString(CultureInfo.InvariantCulture).ParseLongInvariant());
            Assert.AreEqual(long.MaxValue, long.MaxValue.ToString(CultureInfo.InvariantCulture).ParseLongInvariant());
            Assert.AreEqual(0L, "9223372036854775808".ParseLongInvariant());
            Assert.AreEqual(0L, "-9223372036854775809".ParseLongInvariant());
        }

        [Test]
        public void ParseULong_Edges()
        {
            Assert.AreEqual(ulong.MinValue, "0".ParseULongInvariant());
            Assert.AreEqual(ulong.MaxValue, ulong.MaxValue.ToString(CultureInfo.InvariantCulture).ParseULongInvariant());
            Assert.AreEqual(0ul, "-1".ParseULongInvariant());
            Assert.AreEqual(0ul, "18446744073709551616".ParseULongInvariant());
        }

        #endregion

        #region Parse float / double / decimal

        [TestCase("1.5", 1.5)]
        [TestCase("1,5", 1.5)]
        [TestCase(" 1.5 ", 1.5)]
        [TestCase("1 234.5", 1234.5)]
        [TestCase("1 234,5", 1234.5)]
        [TestCase("1.234,56", 1234.56)]
        [TestCase("1,234.56", 1234.56)]
        [TestCase("1'234.5", 1234.5)]
        [TestCase("-1,5", -1.5)]
        [TestCase("+1.5", 1.5)]
        [TestCase("0", 0d)]
        [TestCase("0,0", 0d)]
        public void ParseFloating_SeparatorsAndSpaces(string input, double expected)
        {
            Assert.AreEqual((float)expected, input.ParseFloatInvariant());
            Assert.AreEqual(expected, input.ParseDoubleInvariant());
            Assert.AreEqual((decimal)expected, input.ParseDecimalInvariant());
        }

        [TestCase("1.5e2", 150d)]
        [TestCase("1,5e-1", 0.15d)]
        public void ParseFloatAndDouble_Exponent(string input, double expected)
        {
            Assert.AreEqual((float)expected, input.ParseFloatInvariant(), 1e-6f);
            Assert.AreEqual(expected, input.ParseDoubleInvariant(), 1e-12d);
        }

        [TestCase("abc")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        [TestCase(".")]
        [TestCase(",")]
        public void ParseFloating_Invalid_ReturnsDefault(string input)
        {
            Assert.AreEqual(0f, input.ParseFloatInvariant());
            Assert.AreEqual(-1f, input.ParseFloatInvariant(-1f));
            Assert.AreEqual(0d, input.ParseDoubleInvariant());
            Assert.AreEqual(0m, input.ParseDecimalInvariant());
            Assert.AreEqual(9m, input.ParseDecimalInvariant(9m));
        }

        [Test]
        public void ParseFloat_Edges()
        {
            Assert.AreEqual(float.MaxValue,
                float.MaxValue.ToString("G9", CultureInfo.InvariantCulture).ParseFloatInvariant());
            Assert.AreEqual(float.MinValue,
                float.MinValue.ToString("G9", CultureInfo.InvariantCulture).ParseFloatInvariant());
            Assert.AreEqual(0f, "1e40".ParseFloatInvariant());
            Assert.AreEqual(0f, "-1e40".ParseFloatInvariant());
            Assert.IsTrue(float.IsPositiveInfinity("Infinity".ParseFloatInvariant()));
            Assert.IsTrue(float.IsNegativeInfinity("-Infinity".ParseFloatInvariant()));
            Assert.IsTrue(float.IsNaN("NaN".ParseFloatInvariant()));
        }

        [Test]
        public void ParseDouble_Edges()
        {
            Assert.AreEqual(double.MaxValue,
                double.MaxValue.ToString("G17", CultureInfo.InvariantCulture).ParseDoubleInvariant());
            Assert.AreEqual(double.MinValue,
                double.MinValue.ToString("G17", CultureInfo.InvariantCulture).ParseDoubleInvariant());
            Assert.AreEqual(0d, "1e400".ParseDoubleInvariant());
            Assert.IsTrue(double.IsPositiveInfinity("Infinity".ParseDoubleInvariant()));
            Assert.IsTrue(double.IsNaN("NaN".ParseDoubleInvariant()));
        }

        [Test]
        public void ParseDecimal_Edges()
        {
            Assert.AreEqual(decimal.MaxValue,
                decimal.MaxValue.ToString(CultureInfo.InvariantCulture).ParseDecimalInvariant());
            Assert.AreEqual(decimal.MinValue,
                decimal.MinValue.ToString(CultureInfo.InvariantCulture).ParseDecimalInvariant());
            Assert.AreEqual(0.1m, "0,1".ParseDecimalInvariant());
            Assert.AreEqual(0m, "1e400".ParseDecimalInvariant());
        }

        [Test]
        public void ParseFloating_IgnoresCurrentCulture()
        {
            Assert.AreEqual(1.5f, "1.5".ParseFloatInvariant());
            Assert.AreEqual(1.5d, "1.5".ParseDoubleInvariant());
            Assert.AreEqual(1.5m, "1.5".ParseDecimalInvariant());
        }

        #endregion

        #region ToUpperFirst

        [TestCase("hello", "Hello")]
        [TestCase("Hello", "Hello")]
        [TestCase("a", "A")]
        [TestCase("A", "A")]
        [TestCase("  hello", "  hello")]
        public void ToUpperFirst_ChangesFirstLetter(string input, string expected) =>
            Assert.AreEqual(expected, input.ToUpperFirst());

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        public void ToUpperFirst_NullOrWhiteSpace_ReturnsSame(string input) =>
            Assert.AreEqual(input, input.ToUpperFirst());

        #endregion

        #region GetCountedString

        [TestCase(1)]
        [TestCase(21)]
        [TestCase(31)]
        [TestCase(101)]
        [TestCase(121)]
        public void GetCountedString_One(int count) =>
            Assert.AreEqual("one", count.GetCountedString("one", "many", "more"));

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(22)]
        [TestCase(24)]
        [TestCase(102)]
        public void GetCountedString_Many(int count) =>
            Assert.AreEqual("many", count.GetCountedString("one", "many", "more"));

        [TestCase(0)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(14)]
        [TestCase(19)]
        [TestCase(20)]
        [TestCase(25)]
        [TestCase(111)]
        public void GetCountedString_More(int count) =>
            Assert.AreEqual("more", count.GetCountedString("one", "many", "more"));

        [Test]
        public void GetCountedString_NullTemplate_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 1.GetCountedString(null, "many", "more"));
            Assert.Throws<ArgumentNullException>(() => 1.GetCountedString("one", null, "more"));
            Assert.Throws<ArgumentNullException>(() => 1.GetCountedString("one", "many", null));
            Assert.Throws<ArgumentNullException>(() => 1.GetCountedString(null, "apples"));
            Assert.Throws<ArgumentNullException>(() => 1.GetCountedString("apple", null));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public void GetCountedString_English_One(int count) =>
            Assert.AreEqual("apple", count.GetCountedString("apple", "apples"));

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(100)]
        public void GetCountedString_English_Many(int count) =>
            Assert.AreEqual("apples", count.GetCountedString("apple", "apples"));

        #endregion
    }
}
