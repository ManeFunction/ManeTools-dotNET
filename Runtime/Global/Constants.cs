namespace Mane.DotNet
{
    /// <summary>
    /// Shared numeric constants used by this package.
    /// </summary>
    public static class Mane
    {
        /// <summary>
        /// Default comparison tolerance for <see cref="float"/> values.
        /// Do not use <see cref="float.Epsilon"/> as a comparison tolerance.
        /// </summary>
        public const float FloatTolerance = 1e-6f;

        /// <summary>
        /// Default comparison tolerance for <see cref="double"/> values.
        /// Do not use <see cref="double.Epsilon"/> as a comparison tolerance.
        /// </summary>
        public const double DoubleTolerance = 1e-12d;
    }
}
