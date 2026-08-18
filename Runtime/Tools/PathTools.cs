using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Path-string helpers that keep <c>/</c> and <c>\</c> as written (unlike <see cref="System.IO.Path"/> on Windows).
    /// </summary>
    public static class PathTools
    {
        private static readonly char[] Separators = { '/', '\\' };

        /// <summary>
        /// Removes the last path component. Trailing separators are stripped first.
        /// A root such as <c>/</c> or <c>\</c> is kept.
        /// </summary>
        public static string RemoveLastComponent(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            str = str.TrimEnd(Separators);
            if (str.Length == 0)
                return string.Empty;

            int index = str.LastIndexOfAny(Separators);
            if (index < 0)
                return string.Empty;
            if (index == 0)
                return str[..1];

            return str[..index];
        }
    }
}
