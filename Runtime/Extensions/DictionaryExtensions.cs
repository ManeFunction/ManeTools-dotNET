using System;
using System.Collections.Generic;

namespace Mane.DotNet
{
    /// <summary>
    /// Extension methods for dictionaries.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Convert the dictionary to a new dictionary with new key and value types
        /// </summary>
        public static Dictionary<KO, VO> Convert<KI, VI, KO, VO>(this IReadOnlyDictionary<KI, VI> source,
            Func<KI, VI, (KO, VO)> converter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            
            Dictionary<KO, VO> result = new Dictionary<KO, VO>(source.Count);
            foreach (KeyValuePair<KI, VI> pair in source)
            {
                (KO key, VO value) = converter(pair.Key, pair.Value);
                result.Add(key, value);
            }

            return result;
        }
    
    }
}
