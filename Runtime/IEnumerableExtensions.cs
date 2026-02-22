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
}
