using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Inclusive float interval. <see cref="A"/> and <see cref="B"/> are unordered endpoints;
    /// <see cref="Min"/> and <see cref="Max"/> always return the numeric bounds.
    /// Equality uses <see cref="ManeConst.FloatTolerance"/>.
    /// </summary>
    [Serializable]
    public struct MinMaxFloat : IEquatable<MinMaxFloat>
    {
        /// <summary>
        /// First stored endpoint. May be larger or smaller than <see cref="B"/>.
        /// </summary>
        public float A;

        /// <summary>
        /// Second stored endpoint. May be larger or smaller than <see cref="A"/>.
        /// </summary>
        public float B;

        /// <summary>
        /// Creates an interval from two unordered endpoints.
        /// </summary>
        public MinMaxFloat(float a, float b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Smaller of <see cref="A"/> and <see cref="B"/>.
        /// </summary>
        public readonly float Min => Math.Min(A, B);

        /// <summary>
        /// Larger of <see cref="A"/> and <see cref="B"/>.
        /// </summary>
        public readonly float Max => Math.Max(A, B);

        /// <summary>
        /// Scales both stored endpoints by <paramref name="n"/>.
        /// </summary>
        public static MinMaxFloat operator *(MinMaxFloat value, float n) =>
            new(value.A * n, value.B * n);

        /// <inheritdoc cref="op_Multiply(MinMaxFloat,float)"/>
        public static MinMaxFloat operator *(float n, MinMaxFloat value) =>
            value * n;

        /// <summary>
        /// Divides both stored endpoints by <paramref name="n"/>.
        /// </summary>
        public static MinMaxFloat operator /(MinMaxFloat value, float n) =>
            new(value.A / n, value.B / n);

        /// <summary>
        /// Adds two intervals as [min + min, max + max].
        /// </summary>
        public static MinMaxFloat operator +(MinMaxFloat a, MinMaxFloat b) =>
            new(a.Min + b.Min, a.Max + b.Max);

        /// <summary>
        /// Interval subtraction: [a.min - b.max, a.max - b.min].
        /// </summary>
        public static MinMaxFloat operator -(MinMaxFloat a, MinMaxFloat b) =>
            new(a.Min - b.Max, a.Max - b.Min);

        /// <summary>
        /// Returns true if the normalized bounds are equal within <see cref="ManeConst.FloatTolerance"/>.
        /// </summary>
        public static bool operator ==(MinMaxFloat a, MinMaxFloat b) => a.Equals(b);

        /// <summary>
        /// Returns true if the normalized bounds differ by more than <see cref="ManeConst.FloatTolerance"/>.
        /// </summary>
        public static bool operator !=(MinMaxFloat a, MinMaxFloat b) => !a.Equals(b);

        /// <summary>
        /// Returns true if <paramref name="value"/> lies inside the interval.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <param name="includeMin">When false, the lower bound is exclusive.</param>
        /// <param name="includeMax">When false, the upper bound is exclusive.</param>
        public readonly bool Contains(float value, bool includeMin = true, bool includeMax = true) =>
            (includeMin ? value >= Min : value > Min) && (includeMax ? value <= Max : value < Max);

        /// <summary>
        /// Clamps <paramref name="value"/> to [<see cref="Min"/>, <see cref="Max"/>].
        /// </summary>
        public readonly float Clamp(float value) => Math.Clamp(value, Min, Max);

        /// <summary>
        /// Compares normalized bounds within <see cref="ManeConst.FloatTolerance"/>, so (1, 5) equals (5, 1).
        /// </summary>
        public readonly bool Equals(MinMaxFloat other) =>
            Math.Abs(Min - other.Min) < ManeConst.FloatTolerance &&
            Math.Abs(Max - other.Max) < ManeConst.FloatTolerance;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj is MinMaxFloat other && Equals(other);

        /// <summary>
        /// Hashes normalized bounds snapped to <see cref="ManeConst.FloatTolerance"/>,
        /// so values that compare equal share a hash except on bucket edges.
        /// </summary>
        public readonly override int GetHashCode() =>
            unchecked(Snap(Min).GetHashCode() * 397 ^ Snap(Max).GetHashCode());

        private static long Snap(float value)
        {
            if (float.IsNaN(value))
                return 0;
            if (float.IsPositiveInfinity(value))
                return long.MaxValue;
            if (float.IsNegativeInfinity(value))
                return long.MinValue;

            double scaled = value / ManeConst.FloatTolerance;
            if (double.IsPositiveInfinity(scaled) || scaled >= long.MaxValue)
                return long.MaxValue;
            if (double.IsNegativeInfinity(scaled) || scaled <= long.MinValue)
                return long.MinValue;

            return (long)Math.Round(scaled, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the interval as <c>[Min, Max]</c>.
        /// </summary>
        public readonly override string ToString() => $"[{Min}, {Max}]";
    }
}
