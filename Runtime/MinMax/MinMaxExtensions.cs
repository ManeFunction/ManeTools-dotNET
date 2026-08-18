namespace Mane.DotNet
{
    /// <summary>
    /// Extension methods that treat a value as belonging to a <c>MinMax</c> interval.
    /// </summary>
    public static class MinMaxExtensions
    {
        /// <summary>
        /// Returns true if the value lies inside the interval.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <param name="range">Inclusive interval unless the flags say otherwise.</param>
        /// <param name="includeMin">When false, the lower bound is exclusive.</param>
        /// <param name="includeMax">When false, the upper bound is exclusive.</param>
        public static bool IsInRange(this int value, MinMaxInt range, bool includeMin = true, bool includeMax = true) =>
            range.Contains(value, includeMin, includeMax);

        /// <inheritdoc cref="IsInRange(int,MinMaxInt,bool,bool)"/>
        public static bool IsInRange(this float value, MinMaxFloat range, bool includeMin = true, bool includeMax = true) =>
            range.Contains(value, includeMin, includeMax);

        /// <inheritdoc cref="IsInRange(int,MinMaxInt,bool,bool)"/>
        public static bool IsInRange(this double value, MinMaxDouble range, bool includeMin = true, bool includeMax = true) =>
            range.Contains(value, includeMin, includeMax);

        /// <summary>
        /// Clamps the value to the interval.
        /// </summary>
        public static int Clamp(this int value, MinMaxInt range) => range.Clamp(value);

        /// <inheritdoc cref="Clamp(int,MinMaxInt)"/>
        public static float Clamp(this float value, MinMaxFloat range) => range.Clamp(value);

        /// <inheritdoc cref="Clamp(int,MinMaxInt)"/>
        public static double Clamp(this double value, MinMaxDouble range) => range.Clamp(value);
    }
}
