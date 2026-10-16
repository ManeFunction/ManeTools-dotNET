using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Inclusive float interval. <see cref="A"/> and <see cref="B"/> are unordered endpoints;
    /// <see cref="Min"/> and <see cref="Max"/> always return the numeric bounds.
    /// <c>==</c> compares within <see cref="ManeConst.FloatTolerance"/>; <see cref="Equals(MinMaxFloat)"/> and
    /// <see cref="GetHashCode"/> are exact, so the type works as a dictionary key (like Unity's <c>Vector3</c>).
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
        /// Unlike <see cref="Equals(MinMaxFloat)"/>, this is not exact.
        /// </summary>
        public static bool operator ==(MinMaxFloat a, MinMaxFloat b) =>
            Math.Abs(a.Min - b.Min) < ManeConst.FloatTolerance &&
            Math.Abs(a.Max - b.Max) < ManeConst.FloatTolerance;

        /// <summary>
        /// Returns true if the normalized bounds differ by more than <see cref="ManeConst.FloatTolerance"/>.
        /// </summary>
        public static bool operator !=(MinMaxFloat a, MinMaxFloat b) => !(a == b);

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
        /// Compares normalized bounds exactly, so (1, 5) equals (5, 1). Dictionaries and sets use this;
        /// use <c>==</c> for a comparison within <see cref="ManeConst.FloatTolerance"/>.
        /// </summary>
        public readonly bool Equals(MinMaxFloat other) => Min.Equals(other.Min) && Max.Equals(other.Max);

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj is MinMaxFloat other && Equals(other);

        /// <summary>
        /// Hashes the normalized bounds, consistent with <see cref="Equals(MinMaxFloat)"/>.
        /// </summary>
        public readonly override int GetHashCode() =>
            unchecked(Min.GetHashCode() * 397 ^ Max.GetHashCode());

        /// <summary>
        /// Returns the interval as <c>[Min, Max]</c>.
        /// </summary>
        public readonly override string ToString() => $"[{Min}, {Max}]";
    }
}
