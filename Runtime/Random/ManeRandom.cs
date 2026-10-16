using System;
using System.Security.Cryptography;

namespace Mane.DotNet
{
    /// <summary>
    /// <see cref="IRandom"/> implementation backed by <see cref="Random"/>.
    /// The parameterless constructor seeds from a cryptographically strong source.
    /// Overlapping <see cref="Next"/>, <see cref="Range01"/>, <see cref="Range01Inc"/>, and <see cref="Range01Double"/>
    /// calls are serialized.
    /// </summary>
    public class ManeRandom : IRandom
    {
        private readonly object _sync = new();
        private readonly Random _random;
        private readonly int _seed;

        /// <inheritdoc />
        public int Seed => _seed;

        /// <summary>
        /// Creates a generator with the given seed.
        /// </summary>
        /// <param name="seed">Seed passed to <see cref="Random(int)"/>.</param>
        public ManeRandom(int seed)
        {
            _seed = seed;
            _random = new Random(seed);
        }

        /// <summary>
        /// Creates a generator seeded from <see cref="RandomNumberGenerator"/>.
        /// </summary>
        public ManeRandom()
        {
            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.Fill(bytes);
            _seed = BitConverter.ToInt32(bytes);
            _random = new Random(_seed);
        }

        /// <inheritdoc />
        public int Next(int min, int max)
        {
            lock (_sync)
                return _random.Next(min, max);
        }

        /// <inheritdoc />
        public double Range01Double()
        {
            lock (_sync)
                return _random.NextDouble();
        }

        /// <inheritdoc />
        public float Range01()
        {
            lock (_sync)
                return ToRange01(_random.Next(FloatSteps));
        }

        /// <inheritdoc />
        public float Range01Inc()
        {
            lock (_sync)
                return ToRange01(_random.Next(FloatSteps + 1));
        }

        // A float has 24 bits of precision: below 1 it holds every multiple of 2^-24 exactly. A cast from
        // NextDouble rounds values just under 1 up to 1f.
        internal const int FloatSteps = 1 << 24;

        internal static float ToRange01(int step) => step * (1f / FloatSteps);
    }
}
