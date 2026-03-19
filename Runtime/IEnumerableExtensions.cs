using System.Collections.Generic;

namespace RobsonRocha.UnityCommon
{
    public static class IEnumerableExtensions
    {
        public static void AddIfNotExists<T>(this ICollection<T> values, T item)
        {
            if (!values.Contains(item))
            {
                values.Add(item);
            }
        }
    }

    public static class IDictionaryExtensions
    {
        public static TValue AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.TryGetValue(key, out TValue existingValue))
            {
                dict.Add(key, value);
                return value;
            }
            return existingValue;
        }
    }
}
