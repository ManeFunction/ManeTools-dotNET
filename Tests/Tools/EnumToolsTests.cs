using System;
using System.Linq;
using NUnit.Framework;

namespace Mane.DotNet.Tests
{
    public class EnumToolsTests
    {
        private enum Color
        {
            Red = 1,
            Green = 2,
            [Obsolete]
            Blue = 3
        }

        [Flags]
        private enum Bits
        {
            A = 1,
            B = 2
        }

#pragma warning disable CS0612 // Type or member is obsolete
        [Test]
        public void GetValues_ReturnsAllMembers() =>
            CollectionAssert.AreEqual(new[] { Color.Red, Color.Green, Color.Blue }, EnumTools.GetValues<Color>().ToArray());

        [Test]
        public void IsObsolete_ReadsAttribute()
        {
            Assert.IsFalse(Color.Red.IsObsolete());
            Assert.IsTrue(Color.Blue.IsObsolete());
        }
#pragma warning restore CS0612 // Type or member is obsolete

        [Test]
        public void IsObsolete_CombinedFlagsWithoutField_ReturnsFalse() =>
            Assert.IsFalse((Bits.A | Bits.B).IsObsolete());

        [Test]
        public void IsObsolete_Null_Throws() =>
            Assert.Throws<ArgumentNullException>(() => ((Enum)null).IsObsolete());

        [Test]
        public void ToIntEnum_ParsesNamedAndUndefined()
        {
            Assert.AreEqual(Color.Red, "1".ToIntEnum<Color>());
            Assert.AreEqual((Color)99, "99".ToIntEnum<Color>());
        }

        [Test]
        public void TryParseToIntEnum_DefinedAndFailure()
        {
            Assert.IsTrue("2".TryParseToIntEnum(out Color green));
            Assert.AreEqual(Color.Green, green);

            Assert.IsFalse("99".TryParseToIntEnum(out Color fallback, Color.Red));
            Assert.AreEqual(Color.Red, fallback);

            Assert.IsFalse("abc".TryParseToIntEnum(out Color _, Color.Green));
        }
    }
}
