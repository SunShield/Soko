using System.Collections.Generic;

namespace Soko.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddOrReplace<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key)) dictionary.Remove(key);
            dictionary.Add(key, value);
        }
    }
}