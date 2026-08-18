using System;
using System.Collections.Generic;

namespace Mane.DotNet
{
    /// <summary>
    /// Extension methods for initializing, shuffling, and sampling lists.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Clears the list and fills it with <paramref name="count"/> copies of <paramref name="with"/>.
        /// </summary>
        public static L InitWith<L, T>(this L list, T with, int count) where L : IList<T>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            list.Clear();

            for (int i = 0; i < count; i++)
                list.Add(with);

            return list;
        }
        
        /// <summary>
        /// Clears the list and fills it to its current <see cref="List{T}.Capacity"/> with <paramref name="with"/>.
        /// </summary>
        public static List<T> InitWith<T>(this List<T> list, T with)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            
            return list.InitWith(with, list.Capacity);
        }

        /// <summary>
        /// Clears the list and fills it with <paramref name="count"/> values from the factory.
        /// </summary>
        public static L InitWith<L, T>(this L list, Func<T> with, int count) where L : IList<T>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (with == null)
                throw new ArgumentNullException(nameof(with));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            list.Clear();

            for (int i = 0; i < count; i++)
                list.Add(with());

            return list;
        }

        /// <summary>
        /// Clears the list and fills it with <paramref name="count"/> values from the indexed factory.
        /// </summary>
        public static L InitWith<L, T>(this L list, Func<int, T> with, int count) where L : IList<T>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (with == null)
                throw new ArgumentNullException(nameof(with));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            list.Clear();

            for (int i = 0; i < count; i++)
                list.Add(with(i));

            return list;
        }

        /// <summary>
        /// Clears the list and fills it to its current <see cref="List{T}.Capacity"/> from the factory.
        /// </summary>
        public static List<T> InitWith<T>(this List<T> list, Func<T> with)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            
            return list.InitWith(with, list.Capacity);
        }

        /// <summary>
        /// Clears the list and fills it to its current <see cref="List{T}.Capacity"/> from the indexed factory.
        /// </summary>
        public static List<T> InitWith<T>(this List<T> list, Func<int, T> with)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            
            return list.InitWith(with, list.Capacity);
        }

        /// <summary>
        /// Shuffle the list
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, IRandom random)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Returns a random element, or default if the list is empty.
        /// </summary>
        /// <param name="list">Source list.</param>
        /// <param name="selectedIdx">Index of the chosen element, or -1 when the list is empty.</param>
        /// <param name="random">Random source.</param>
        public static T RandomOrDefaultWithIdx<T>(this IReadOnlyList<T> list, out int selectedIdx, IRandom random)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            if (list.Count == 0)
            {
                selectedIdx = -1;
                return default;
            }

            selectedIdx = random.Next(0, list.Count);

            return list[selectedIdx];
        }
    }
}