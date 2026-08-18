namespace Mane.DotNet
{
    /// <summary>
    /// Ring buffer of recent <see cref="double"/> values.
    /// </summary>
    public class DoubleHistoryCache : HistoryCache<double>
    {
        /// <summary>
        /// Creates a cache that stores up to <paramref name="length"/> values.
        /// </summary>
        /// <param name="length">Buffer size. Must be greater than 0.</param>
        public DoubleHistoryCache(int length) : base(length) { }

        /// <inheritdoc />
        public override double GetAverage()
        {
            if (Count == 0)
                return 0d;

            double sum = 0d;
            for (int i = 0; i < Count; i++)
                sum += History[i];

            return sum / Count;
        }
    }
}
