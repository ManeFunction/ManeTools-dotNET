using System;
using System.Globalization;
using System.Text;

namespace Mane.DotNet
{
    /// <summary>
    /// String helpers: culture-invariant number parsing, capitalization, and counted nouns.
    /// </summary>
    public static class StringExtensions
    {
        private delegate bool TryParseNumber<T>(string s, NumberStyles style, IFormatProvider provider, out T result);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static sbyte ParseSByteInvariant(this string str, sbyte defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, sbyte.TryParse, sbyte.MinValue, sbyte.MaxValue, v => (sbyte)v);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static byte ParseByteInvariant(this string str, byte defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, byte.TryParse, byte.MinValue, byte.MaxValue, v => (byte)v);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static short ParseShortInvariant(this string str, short defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, short.TryParse, short.MinValue, short.MaxValue, v => (short)v);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static ushort ParseUShortInvariant(this string str, ushort defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, ushort.TryParse, ushort.MinValue, ushort.MaxValue, v => (ushort)v);

        /// <summary>
        /// Parses the string ignoring the current culture. Whitespace and apostrophes are removed anywhere in the string,
        /// so they work as thousands separators (<c>1 234</c>, <c>1'234</c>).
        /// When both <c>.</c> and <c>,</c> appear, the last mark is the decimal separator and every other mark is a
        /// thousands separator (<c>1,234.5</c>, <c>1.234,5</c>). When only one kind appears, it is the decimal separator
        /// if it occurs once and a thousands separator if it repeats (<c>1.234.567</c>).
        /// Integers also accept a whole number written with a decimal part of zeros, so <c>1.000</c> is 1, not 1000.
        /// Returns <paramref name="defaultValue"/> when the string is null, empty, not a valid number, out of range,
        /// or has a non-zero decimal part.
        /// </summary>
        public static int ParseIntInvariant(this string str, int defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, int.TryParse, int.MinValue, int.MaxValue, v => (int)v);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static uint ParseUIntInvariant(this string str, uint defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, uint.TryParse, uint.MinValue, uint.MaxValue, v => (uint)v);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static long ParseLongInvariant(this string str, long defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, long.TryParse, long.MinValue, long.MaxValue, v => (long)v);

        /// <inheritdoc cref="ParseIntInvariant"/>
        public static ulong ParseULongInvariant(this string str, ulong defaultValue = 0) =>
            ParseIntegerInvariant(str, defaultValue, ulong.TryParse, ulong.MinValue, ulong.MaxValue, v => (ulong)v);

        /// <summary>
        /// Parses the string ignoring the current culture. Whitespace and apostrophes are removed anywhere in the string,
        /// so they work as thousands separators (<c>1 234.5</c>, <c>1'234.5</c>).
        /// When both <c>.</c> and <c>,</c> appear, the last mark is the decimal separator and every other mark is a
        /// thousands separator (<c>1,234.5</c>, <c>1.234,5</c>). When only one kind appears, it is the decimal separator
        /// if it occurs once and a thousands separator if it repeats (<c>1.234.567</c>).
        /// Returns <paramref name="defaultValue"/> when the string is null, empty, or not a valid number.
        /// </summary>
        public static float ParseFloatInvariant(this string str, float defaultValue = 0f) =>
            ParseInvariant(str, defaultValue, NumberStyles.Float, float.TryParse);

        /// <inheritdoc cref="ParseFloatInvariant"/>
        public static double ParseDoubleInvariant(this string str, double defaultValue = 0d) =>
            ParseInvariant(str, defaultValue, NumberStyles.Float, double.TryParse);

        /// <inheritdoc cref="ParseFloatInvariant"/>
        public static decimal ParseDecimalInvariant(this string str, decimal defaultValue = 0m) =>
            ParseInvariant(str, defaultValue, NumberStyles.Number, decimal.TryParse);

        private static T ParseIntegerInvariant<T>(
            string str,
            T defaultValue,
            TryParseNumber<T> tryParse,
            decimal min,
            decimal max,
            Func<decimal, T> convert)
        {
            if (!TryNormalizeNumber(str, out string normalized))
                return defaultValue;

            if (tryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out T result))
                return result;

            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
                && value >= min && value <= max
                && value == decimal.Truncate(value))
                return convert(value);

            return defaultValue;
        }

        private static T ParseInvariant<T>(
            string str,
            T defaultValue,
            NumberStyles style,
            TryParseNumber<T> tryParse)
        {
            if (!TryNormalizeNumber(str, out string normalized))
                return defaultValue;

            return tryParse(normalized, style, CultureInfo.InvariantCulture, out T result)
                ? result
                : defaultValue;
        }

        private static bool TryNormalizeNumber(string str, out string normalized)
        {
            normalized = null;
            if (string.IsNullOrWhiteSpace(str))
                return false;

            StringBuilder compact = new StringBuilder(str.Length);
            int lastDot = -1;
            int lastComma = -1;
            int dots = 0;
            int commas = 0;

            foreach (char c in str)
            {
                if (char.IsWhiteSpace(c) || c == '\'' || c == '\u2019')
                    continue;

                if (c == '.')
                {
                    lastDot = compact.Length;
                    dots++;
                }
                else if (c == ',')
                {
                    lastComma = compact.Length;
                    commas++;
                }

                compact.Append(c);
            }

            if (compact.Length == 0)
                return false;

            int decimalIndex = -1;
            if (dots > 0 && commas > 0)
                decimalIndex = lastDot > lastComma ? lastDot : lastComma;
            else if (dots == 1)
                decimalIndex = lastDot;
            else if (commas == 1)
                decimalIndex = lastComma;

            StringBuilder result = new StringBuilder(compact.Length);
            for (int i = 0; i < compact.Length; i++)
            {
                char c = compact[i];
                if (c is '.' or ',')
                {
                    if (i == decimalIndex)
                        result.Append('.');
                    continue;
                }

                result.Append(c);
            }

            if (result.Length == 0)
                return false;

            normalized = result.ToString();
            return true;
        }
        
        /// <summary>
        /// Converts the first character of the string to uppercase with the casing rules of
        /// <paramref name="culture"/>, or of the current culture when it is null, like <see cref="string.ToUpper()"/>.
        /// In a Unity player the current culture is the player's OS locale, not the game language: under Turkish,
        /// "item" becomes "İtem". Use <see cref="ToFirstUpperInvariant"/> for text that must not depend on it.
        /// </summary>
        public static string ToFirstUpper(this string str, CultureInfo culture = null) =>
            ToFirstUpper(str, (culture ?? CultureInfo.CurrentCulture).TextInfo);

        /// <summary>
        /// Converts the first character of the string to uppercase with the invariant culture's casing rules,
        /// so the result is the same on every machine ("item" is always "Item").
        /// </summary>
        public static string ToFirstUpperInvariant(this string str) =>
            ToFirstUpper(str, CultureInfo.InvariantCulture.TextInfo);

        private static string ToFirstUpper(string str, TextInfo textInfo)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            // A letter outside the Basic Multilingual Plane is a surrogate pair, uppercased as one unit.
            int length = str.Length > 1 && char.IsSurrogatePair(str[0], str[1]) ? 2 : 1;
            return textInfo.ToUpper(str[..length]) + str[length..];
        }
        
        /// <summary>
        /// Slavic-style plural form: 1 → <paramref name="one"/>, 2–4 → <paramref name="many"/>,
        /// 5+ and teens 11–19 → <paramref name="more"/>.
        /// </summary>
        /// <param name="count">Count used to pick the form. The sign is ignored: -21 uses the same form as 21.</param>
        /// <param name="one">Form for 1, 21, 31, …</param>
        /// <param name="many">Form for 2–4, 22–24, …</param>
        /// <param name="more">Form for 0, 5–20, 25–30, …</param>
        public static string GetCountedString(this int count, string one, string many, string more)
        {
            if (one == null)
                throw new ArgumentNullException(nameof(one));
            if (many == null)
                throw new ArgumentNullException(nameof(many));
            if (more == null)
                throw new ArgumentNullException(nameof(more));

            // Abs of the remainder, not of the count: Math.Abs(int.MinValue) overflows.
            int tens = Math.Abs(count % 100);
            if (tens is > 5 and < 20)
                return more;

            int units = Math.Abs(count % 10);
            if (units == 1)
                return one;
            
            if (units is > 1 and < 5)
                return many;
            return more;
        }

        /// <summary>
        /// English-style plural form: <paramref name="count"/> greater than 1 uses <paramref name="many"/>,
        /// otherwise <paramref name="one"/>.
        /// </summary>
        /// <param name="count">Count used to pick the form.</param>
        /// <param name="one">Form for 1, 0, and negative counts.</param>
        /// <param name="many">Form for counts greater than 1.</param>
        public static string GetCountedString(this int count, string one, string many)
        {
            if (one == null)
                throw new ArgumentNullException(nameof(one));
            if (many == null)
                throw new ArgumentNullException(nameof(many));

            return count > 1 ? many : one;
        }
    }
}