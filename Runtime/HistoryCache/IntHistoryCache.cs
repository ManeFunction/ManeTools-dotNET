using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Ring buffer of recent <see cref="int"/> values.
    /// <see cref="HistoryCache{T}.GetAverage"/> returns the mean rounded to the nearest
    /// <see cref="int"/>, with midpoints away from zero.
    /// </summary>
    public class IntHistoryCache : HistoryCache<int>
    {
        /// <summary>
        /// Creates a cache that stores up to <paramref name="length"/> values.
        /// </summary>
        /// <param name="length">Buffer size. Must be greater than 0.</param>
        public IntHistoryCache(int length) : base(length) { }

        /// <inheritdoc />
        protected override int ComputeAverage()
        {
            if (Count == 0)
                return 0;

            long sum = 0L;
            for (int i = 0; i < Count; i++)
                sum += History[i];

            return (int)Math.Round(sum / (double)Count, MidpointRounding.AwayFromZero);
        }
    }
}
