using System.Collections.Generic;

namespace Assets.Scripts.Utility
{
    public static class DictionaryUtils
    {
        public static V At<K, V>(this Dictionary<K, V> dictionary, K key) where V : class
        {
            return dictionary.TryGetValue(key, out V result) ? result : null;
        }
    }
}
