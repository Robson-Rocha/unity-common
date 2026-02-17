using System;
using System.Collections.Generic;

public static class IEnumerableOfBooleanExtensions
{
    public static bool All(this IEnumerable<bool> enumerable)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable), "The enumerable cannot be null.");
        }
        foreach (bool item in enumerable)
        {
            if (!item)
            {
                return false;
            }
        }
        return true;
    }

    public static bool Any(this IEnumerable<bool> enumerable)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable), "The enumerable cannot be null.");
        }
        foreach (bool item in enumerable)
        {
            if (item)
            {
                return true;
            }
        }
        return false;
    }
}
