using System;
using System.Collections.Generic;
using System.Linq;

namespace Mane.DotNet
{
    /// <summary>
    /// Helpers for working with enum types.
    /// </summary>
    public static class EnumTools
    {
        /// <summary>
        /// Returns all values of the enum type.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        public static IEnumerable<T> GetValues<T>() where T : Enum =>
            Enum.GetValues(typeof(T)).Cast<T>();

        /// <summary>
        /// Returns true if the enum member is marked with <see cref="ObsoleteAttribute"/>.
        /// Combined flags with no matching field name return false.
        /// </summary>
        public static bool IsObsolete(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var fi = value.GetType().GetField(value.ToString());
            if (fi == null)
                return false;

            var attributes = (ObsoleteAttribute[])
                fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return attributes.Any();
        }

        /// <summary>
        /// Parses a decimal integer string and converts it to the enum type.
        /// Does not require the value to be a named member.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        public static T ToIntEnum<T>(this string value) where T : struct, Enum =>
            (T)Enum.ToObject(typeof(T), int.Parse(value));

        /// <summary>
        /// Tries to parse a decimal integer string as a named member of the enum.
        /// Returns <paramref name="defaultValue"/> when the parse fails or the value is not defined.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="value">Integer string to parse.</param>
        /// <param name="result">Parsed enum, or <paramref name="defaultValue"/> on failure.</param>
        /// <param name="defaultValue">Value to use when parsing fails.</param>
        /// <returns>True if the string is a defined member of <typeparamref name="T"/>.</returns>
        public static bool TryParseToIntEnum<T>(this string value, out T result, T defaultValue = default) where T : struct, Enum
        {
            if (int.TryParse(value, out int intValue) && Enum.IsDefined(typeof(T), intValue))
            {
                result = (T)Enum.ToObject(typeof(T), intValue);
                return true;
            }
            result = defaultValue;
            return false;
        }
    }
}
