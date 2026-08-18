using System;

namespace Mane.DotNet
{
    /// <summary>
    /// Extension methods for filling and clearing arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Fills every element with <paramref name="with"/> and returns the same array.
        /// </summary>
        public static T[] FillWith<T>(this T[] array, T with)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            for (int i = 0; i < array.Length; i++)
                array[i] = with;

            return array;
        }

        /// <summary>
        /// Fills every element by invoking the factory and returns the same array.
        /// </summary>
        public static T[] FillWith<T>(this T[] array, Func<T> with)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (with == null)
                throw new ArgumentNullException(nameof(with));

            for (int i = 0; i < array.Length; i++)
                array[i] = with();

            return array;
        }

        /// <summary>
        /// Fills every element by invoking the factory with the element index and returns the same array.
        /// </summary>
        public static T[] FillWith<T>(this T[] array, Func<int, T> with)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (with == null)
                throw new ArgumentNullException(nameof(with));

            for (int i = 0; i < array.Length; i++)
                array[i] = with(i);

            return array;
        }

        /// <inheritdoc cref="FillWith{T}(T[],T)"/>
        public static T[,] FillWith<T>(this T[,] array, T with)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int w = array.GetLength(0);
            int h = array.GetLength(1);
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    array[i, j] = with;

            return array;
        }

        /// <inheritdoc cref="FillWith{T}(T[],Func{T})"/>
        public static T[,] FillWith<T>(this T[,] array, Func<T> with)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (with == null)
                throw new ArgumentNullException(nameof(with));

            int w = array.GetLength(0);
            int h = array.GetLength(1);
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    array[i, j] = with();

            return array;
        }

        /// <summary>
        /// Fills every element by invoking the factory with (x, y) indices and returns the same array.
        /// </summary>
        public static T[,] FillWith<T>(this T[,] array, Func<int, int, T> with)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (with == null)
                throw new ArgumentNullException(nameof(with));

            int w = array.GetLength(0);
            int h = array.GetLength(1);
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    array[i, j] = with(i, j);

            return array;
        }

        /// <summary>
        /// Clears the array
        /// </summary>
        public static void Clear<T>(this T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            
            Array.Clear(array, 0, array.Length);
        }

        /// <summary>
        /// Clears the array
        /// </summary>
        public static void Clear<T>(this T[,] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            
            Array.Clear(array, 0, array.Length);
        }
    }
}