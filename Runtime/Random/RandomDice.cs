using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Rolling a dice with a custom number of sides.
    /// Absolutely unnecessary, but makes your coding experience more fun.
    /// </summary>
    public static class RandomDice
    {
        /// <summary>
        /// Rolls an n-sided die and returns a value in [1, <paramref name="sides"/>].
        /// </summary>
        /// <param name="sides">Number of faces. Must be at least 1.</param>
        /// <param name="random">Random source.</param>
        public static int Roll(int sides, IRandom random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));
            if (sides < 1)
                throw new ArgumentOutOfRangeException(nameof(sides));

            return random.Next(1, sides + 1);
        }

        /// <summary>Rolls a 4-sided die.</summary>
        public static int Roll4(IRandom random) => Roll(4, random);

        /// <summary>Rolls a 6-sided die.</summary>
        public static int Roll6(IRandom random) => Roll(6, random);

        /// <summary>Rolls an 8-sided die.</summary>
        public static int Roll8(IRandom random) => Roll(8, random);

        /// <summary>Rolls a 10-sided die.</summary>
        public static int Roll10(IRandom random) => Roll(10, random);

        /// <summary>Rolls a 12-sided die.</summary>
        public static int Roll12(IRandom random) => Roll(12, random);

        /// <summary>Rolls a 20-sided die.</summary>
        public static int Roll20(IRandom random) => Roll(20, random);
    }

    /// <summary>
    /// Flipping a coin.
    /// Absolutely unnecessary, but makes your coding experience more fun.
    /// </summary>
    public static class RandomCoin
    {
        /// <summary>
        /// Returns true or false with equal probability.
        /// </summary>
        public static bool Flip(IRandom random) => RandomDice.Roll(2, random) == 1;
    }
}
