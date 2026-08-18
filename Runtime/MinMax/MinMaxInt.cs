using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Inclusive integer interval. <see cref="A"/> and <see cref="B"/> are unordered endpoints;
    /// <see cref="Min"/> and <see cref="Max"/> always return the numeric bounds.
    /// Arithmetic on <see cref="int"/> endpoints is checked and throws on overflow.
    /// </summary>
    [Serializable]
    public struct MinMaxInt : IEquatable<MinMaxInt>
    {
        /// <summary>
        /// First stored endpoint. May be larger or smaller than <see cref="B"/>.
        /// </summary>
        public int A;

        /// <summary>
        /// Second stored endpoint. May be larger or smaller than <see cref="A"/>.
        /// </summary>
        public int B;

        /// <summary>
        /// Creates an interval from two unordered endpoints.
        /// </summary>
        public MinMaxInt(int a, int b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Smaller of <see cref="A"/> and <see cref="B"/>.
        /// </summary>
        public readonly int Min => Math.Min(A, B);

        /// <summary>
        /// Larger of <see cref="A"/> and <see cref="B"/>.
        /// </summary>
        public readonly int Max => Math.Max(A, B);

        /// <summary>
        /// Scales both stored endpoints by <paramref name="n"/>.
        /// </summary>
        public static MinMaxInt operator *(MinMaxInt value, int n)
        {
            checked
            {
                return new MinMaxInt(value.A * n, value.B * n);
            }
        }

        /// <inheritdoc cref="op_Multiply(MinMaxInt,int)"/>
        public static MinMaxInt operator *(int n, MinMaxInt value) =>
            value * n;

        /// <summary>
        /// Divides both stored endpoints by <paramref name="n"/> using integer division.
        /// </summary>
        public static MinMaxInt operator /(MinMaxInt value, int n) =>
            new(value.A / n, value.B / n);

        /// <summary>
        /// Adds two intervals as [min + min, max + max].
        /// </summary>
        public static MinMaxInt operator +(MinMaxInt a, MinMaxInt b)
        {
            checked
            {
                return new MinMaxInt(a.Min + b.Min, a.Max + b.Max);
            }
        }

        /// <summary>
        /// Interval subtraction: [a.min - b.max, a.max - b.min].
        /// </summary>
        public static MinMaxInt operator -(MinMaxInt a, MinMaxInt b)
        {
            checked
            {
                return new MinMaxInt(a.Min - b.Max, a.Max - b.Min);
            }
        }

        /// <summary>
        /// Returns true if the normalized bounds are equal.
        /// </summary>
        public static bool operator ==(MinMaxInt a, MinMaxInt b) => a.Equals(b);

        /// <summary>
        /// Returns true if the normalized bounds differ.
        /// </summary>
        public static bool operator !=(MinMaxInt a, MinMaxInt b) => !a.Equals(b);

        /// <summary>
        /// Returns true if <paramref name="value"/> lies inside the interval.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <param name="includeMin">When false, the lower bound is exclusive.</param>
        /// <param name="includeMax">When false, the upper bound is exclusive.</param>
        public readonly bool Contains(int value, bool includeMin = true, bool includeMax = true) =>
            (includeMin ? value >= Min : value > Min) && (includeMax ? value <= Max : value < Max);

        /// <summary>
        /// Clamps <paramref name="value"/> to [<see cref="Min"/>, <see cref="Max"/>].
        /// </summary>
        public readonly int Clamp(int value) => Math.Clamp(value, Min, Max);

        /// <summary>
        /// Compares normalized <see cref="Min"/> and <see cref="Max"/>, so (1, 5) equals (5, 1).
        /// </summary>
        public readonly bool Equals(MinMaxInt other) => Min == other.Min && Max == other.Max;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj is MinMaxInt other && Equals(other);

        /// <inheritdoc />
        public readonly override int GetHashCode() =>
            unchecked(Min.GetHashCode() * 397 ^ Max.GetHashCode());

        /// <summary>
        /// Returns the interval as <c>[Min, Max]</c>.
        /// </summary>
        public readonly override string ToString() => $"[{Min}, {Max}]";
    }
}
