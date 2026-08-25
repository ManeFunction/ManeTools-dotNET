namespace Mane.DotNet
{
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