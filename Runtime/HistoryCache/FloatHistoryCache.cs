namespace Mane.DotNet
{
    /// <summary>
    /// Ring buffer of recent <see cref="float"/> values.
    /// </summary>
    public class FloatHistoryCache : HistoryCache<float>
    {
        /// <summary>
        /// Creates a cache that stores up to <paramref name="length"/> values.
        /// </summary>
        /// <param name="length">Buffer size. Must be greater than 0.</param>
        public FloatHistoryCache(int length) : base(length) { }

        /// <inheritdoc />
        public override float GetAverage()
        {
            if (Count == 0)
                return 0f;

            double sum = 0d;
            for (int i = 0; i < Count; i++)
                sum += History[i];

            return (float)(sum / Count);
        }
    }
}
