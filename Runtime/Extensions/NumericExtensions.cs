using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Extension methods for clamping, remapping, parity, and saturating integer arithmetic.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Clamps the value to [<paramref name="min"/>, <paramref name="max"/>].
        /// Throws if <paramref name="min"/> is greater than <paramref name="max"/>.
        /// </summary>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentException($"{nameof(min)} must be less than or equal to {nameof(max)}.", nameof(min));

            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }
        
        /// <summary>
        /// Clamp the value to the min
        /// </summary>
        public static T ClampMin<T>(this T value, T min) where T : IComparable<T> => 
            value.CompareTo(min) < 0 ? min : value;
        
        /// <summary>
        /// Clamp the value to the max
        /// </summary>
        public static T ClampMax<T>(this T value, T max) where T : IComparable<T> => 
            value.CompareTo(max) > 0 ? max : value;

        /// <summary>
        /// Clamp the value to the 0-1 range
        /// </summary>
        public static float Clamp01(this float value) => Math.Clamp(value, 0f, 1f);

        /// <summary>
        /// Clamp the value to the 0-1 range
        /// </summary>
        public static double Clamp01(this double value) => Math.Clamp(value, 0d, 1d);

        /// <summary>
        /// Clamp the value to the 0-1 range
        /// </summary>
        public static decimal Clamp01(this decimal value) => Math.Clamp(value, 0m, 1m);

        /// <summary>
        /// Check if the value is even
        /// </summary>
        public static bool IsEven(this int value) => (value >> 1) << 1 == value;

        /// <summary>
        /// Check if the value is odd
        /// </summary>
        public static bool IsOdd(this int value) => !IsEven(value);

        /// <summary>
        /// Linearly maps a value from [sourceFrom, sourceTo] onto [destFrom, destTo].
        /// A zero-width source range returns <paramref name="destFrom"/>.
        /// </summary>
        public static float Remap(this float value, float sourceFrom, float sourceTo, float destFrom, float destTo) =>
            Math.Abs(sourceTo - sourceFrom) < Mane.FloatTolerance
                ? destFrom
                : (value - sourceFrom) / (sourceTo - sourceFrom) * (destTo - destFrom) + destFrom;
        
        /// <summary>
        /// Linearly maps a value from [sourceFrom, sourceTo] onto [destFrom, destTo].
        /// A zero-width source range returns <paramref name="destFrom"/>.
        /// </summary>
        public static double Remap(this double value, double sourceFrom, double sourceTo, double destFrom, double destTo) =>
            Math.Abs(sourceTo - sourceFrom) < Mane.DoubleTolerance
                ? destFrom
                : (value - sourceFrom) / (sourceTo - sourceFrom) * (destTo - destFrom) + destFrom;

        /// <summary>
        /// Adds without wrapping. Overflow saturates to <see cref="int.MaxValue"/> or <see cref="int.MinValue"/>.
        /// </summary>
        public static int SafePlus(this int value, int add)
        {
            try
            {
                return checked(value + add);
            }
            catch (OverflowException)
            {
                return add > 0 ? int.MaxValue : int.MinValue;
            }
        }
        
        /// <summary>
        /// Subtracts without wrapping. Overflow saturates to <see cref="int.MaxValue"/> or <see cref="int.MinValue"/>.
        /// </summary>
        public static int SafeMinus(this int value, int remove)
        {
            try
            {
                return checked(value - remove);
            }
            catch (OverflowException)
            {
                return remove > 0 ? int.MinValue : int.MaxValue;
            }
        }
        
        /// <summary>
        /// Multiplies without wrapping. Overflow saturates to <see cref="int.MaxValue"/> or <see cref="int.MinValue"/> by product sign.
        /// </summary>
        public static int SafeMultiply(this int value, int multiplier)
        {
            try
            {
                return checked(value * multiplier);
            }
            catch (OverflowException)
            {
                return (value < 0) == (multiplier < 0) ? int.MaxValue : int.MinValue;
            }
        }
    }
}