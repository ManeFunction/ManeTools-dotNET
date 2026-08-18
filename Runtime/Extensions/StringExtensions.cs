using System;
using System.Globalization;
using System.Text;

namespace Mane.DotNet
{
    /// <summary>
    /// Culture-invariant string helpers: number parsing, capitalization, and counted nouns.
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
        /// Parses the string ignoring the current culture. Trims whitespace and apostrophes.
        /// The last <c>.</c> or <c>,</c> is the decimal separator; extra marks of the same kind are thousands separators.
        /// Integers also accept a whole number written with a decimal part of zeros (for example <c>1.000</c>).
        /// Returns <paramref name="defaultValue"/> when the string is null, empty, or not a valid number.
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
        /// Parses the string ignoring the current culture. Trims whitespace and apostrophes.
        /// The last <c>.</c> or <c>,</c> is the decimal separator; extra marks of the same kind are thousands separators.
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
        /// Convert the first character of the string to uppercase
        /// </summary>
        public static string ToUpperFirst(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            
            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString(); 
            
            return char.ToUpper(str[0]) + str[1..];
        }
        
        /// <summary>
        /// Slavic-style plural form: 1 → <paramref name="one"/>, 2–4 → <paramref name="many"/>,
        /// 5+ and teens 11–19 → <paramref name="more"/>.
        /// </summary>
        /// <param name="count">Count used to pick the form. The sign is ignored via modulo.</param>
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

            int tens = count % 100;
            if (tens is > 5 and < 20)
                return more;
            
            int units = count % 10;
            if (units == 1)
                return one;
            
            if (units is > 1 and < 5)
                return many;
            return more;
        }
    }
}