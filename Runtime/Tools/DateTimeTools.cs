using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Helpers for comparing <see cref="DateTime"/> values.
    /// </summary>
    public static class DateTimeTools
    {
        /// <summary>
        /// Returns the later of two dates.
        /// </summary>
        public static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

        /// <summary>
        /// Returns the earlier of two dates.
        /// </summary>
        public static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
    }
}
