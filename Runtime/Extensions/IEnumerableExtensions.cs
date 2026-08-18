using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mane.DotNet
{
    /// <summary>
    /// Extension methods for enumerables: iteration, sampling, and emptiness checks.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Invokes <paramref name="breakFunc"/> for each element until it returns false.
        /// </summary>
        public static void ForEachCancellable<T>(this IEnumerable<T> list, Func<T, bool> breakFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (breakFunc == null)
                throw new ArgumentNullException(nameof(breakFunc));

            foreach (T element in list)
                if (!breakFunc(element))
                    break;
        }
        
        /// <summary>
        /// Execute the action for each element in the list
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (T element in list)
                action(element);
        }

        /// <summary>
        /// Returns up to <paramref name="count"/> random elements from the source.
        /// Copies the collection first, so this is expensive for large inputs.
        /// By default the result has no duplicates. When <paramref name="getMaxWithDuplicates"/>
        /// is true and the source is smaller than <paramref name="count"/>, the whole source
        /// is repeated until the requested length is reached, then the remainder is sampled without duplicates.
        /// </summary>
        public static List<T> GetRandomList<T>(this IEnumerable<T> collection, int count,
            IRandom random, bool getMaxWithDuplicates = false)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (random == null)
                throw new ArgumentNullException(nameof(random));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            List<T> result = new(count);
            if (count == 0)
                return result;

            List<T> pool = new(collection);
            if (pool.Count == 0)
                return result;

            int take = count;
            if (getMaxWithDuplicates)
            {
                while (pool.Count < take)
                {
                    result.AddRange(pool);
                    take -= pool.Count;
                }
            }
            else
                take = Math.Min(pool.Count, count);

            int remaining = pool.Count;
            for (int i = 0; i < take; i++)
            {
                int idx = random.Next(0, remaining);
                result.Add(pool[idx]);
                remaining--;
                pool[idx] = pool[remaining];
            }

            return result;
        }
        
        /// <summary>
        /// Return a random element from the collection or default if the collection is empty
        /// </summary>
        public static T RandomOrDefault<T>(this IEnumerable<T> collection, IRandom random)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (random == null)
                throw new ArgumentNullException(nameof(random));
            
            if (collection is IReadOnlyList<T> readOnlyList)
                return readOnlyList.RandomOrDefaultWithIdx(out _, random);

            if (collection is IList<T> list)
            {
                if (list.Count == 0)
                    return default;

                return list[random.Next(0, list.Count)];
            }

            if (collection is IReadOnlyCollection<T> readOnlyCollection)
            {
                int n = readOnlyCollection.Count;
                return n == 0 ? default : collection.ElementAtOrDefault(random.Next(0, n));
            }

            if (collection is ICollection<T> countable)
            {
                int n = countable.Count;
                return n == 0 ? default : collection.ElementAtOrDefault(random.Next(0, n));
            }

            T selected = default;
            int count = 0;
            foreach (T item in collection)
            {
                count++;
                if (random.Next(0, count) == 0)
                    selected = item;
            }

            return selected;
        }

        /// <summary>
        /// Concatenate the list with itself <paramref name="times"/> times
        /// </summary>
        public static IEnumerable<T> SelfConcat<T>(this IEnumerable<T> list, int times)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (times < 0)
                throw new ArgumentOutOfRangeException(nameof(times));
            
            if (times == 0)
                return Enumerable.Empty<T>();
            
            if (times == 1)
                return list;

            IEnumerable<T> res = list;
            for (int i = 1; i < times; i++)
                res = res.Concat(list);
            
            return res;
        }

        /// <summary>
        /// Returns true if at least <paramref name="count"/> elements match the predicate.
        /// </summary>
        public static bool AnyAtLeast<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            int n = 0;
            
            return source.Any(element => predicate(element) && ++n == count);
        }

        /// <summary>
        /// Check if the collection is null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return true;

            switch (collection)
            {
                case ICollection<T> c:
                    return c.Count == 0;
                case IReadOnlyCollection<T> c:
                    return c.Count == 0;
                case ICollection c:
                    return c.Count == 0;
                case string s:
                    return s.Length == 0;
                default:
                    return !collection.Any();
            }
        }
    }
}
