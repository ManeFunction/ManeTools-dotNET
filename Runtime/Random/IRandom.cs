namespace Mane.DotNet
{
    /// <summary>
    /// Seeded random source used by this package.
    /// <see cref="Next(int,int)"/> uses the same bounds as <see cref="System.Random.Next(int,int)"/>:
    /// the lower bound is inclusive and the upper bound is exclusive.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Seed of this generator.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Returns a random integer in the range [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        int Next(int min, int max);

        /// <summary>
        /// Returns a random double in the range [0, 1).
        /// </summary>
        double Range01Double();

        /// <summary>
        /// Returns a random float in the range [0, 1).
        /// </summary>
        float Range01();
    }
}
