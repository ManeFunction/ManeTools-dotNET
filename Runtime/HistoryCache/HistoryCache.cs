using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Fixed-length ring buffer of recent values.
    /// Overlapping <see cref="Append"/>, <see cref="Clear"/>, and <see cref="GetAverage"/> calls are serialized.
    /// </summary>
    /// <typeparam name="T">Stored value type.</typeparam>
    public abstract class HistoryCache<T>
    {
        /// <summary>
        /// Backing buffer. Filled from index 0 up to <see cref="Count"/>, then overwritten in a ring.
        /// Read it only from <see cref="ComputeAverage"/>, which runs under the cache lock.
        /// </summary>
        protected readonly T[] History;

        private readonly object _sync = new();
        private int _idx;
        private int _count;

        /// <summary>
        /// Creates a cache that stores up to <paramref name="length"/> values.
        /// </summary>
        /// <param name="length">Buffer size. Must be greater than 0.</param>
        protected HistoryCache(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            History = new T[length];
        }

        /// <summary>
        /// Number of values written so far, up to the buffer length.
        /// Read it only from <see cref="ComputeAverage"/>, which runs under the cache lock.
        /// </summary>
        protected int Count => _count;

        /// <summary>
        /// Average of the values currently stored. Empty cache returns the type's zero.
        /// </summary>
        public T GetAverage()
        {
            lock (_sync)
                return ComputeAverage();
        }

        /// <summary>
        /// Computes the average. Called by <see cref="GetAverage"/> while the cache lock is held.
        /// </summary>
        protected abstract T ComputeAverage();

        /// <summary>
        /// Appends a value, overwriting the oldest entry once the buffer is full.
        /// </summary>
        /// <param name="value">Value to store.</param>
        public void Append(T value)
        {
            lock (_sync)
            {
                History[_idx] = value;
                _idx++;
                if (_idx == History.Length)
                    _idx = 0;
                if (_count < History.Length)
                    _count++;
            }
        }

        /// <summary>
        /// Removes all stored values.
        /// </summary>
        public void Clear()
        {
            lock (_sync)
            {
                History.Clear();
                _idx = 0;
                _count = 0;
            }
        }
    }
}
