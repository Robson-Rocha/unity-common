using System;
using System.Collections.Generic;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Provides optimized boolean aggregation helpers for <see cref="IEnumerable{Boolean}"/>.
    /// </summary>
    public static class IEnumerableOfBooleanExtensions
    {
        /// <summary>
        /// Determines whether all values in the sequence are <see langword="true"/>.
        /// </summary>
        /// <param name="enumerable">The sequence to evaluate.</param>
        /// <returns><see langword="true"/> if every value is true; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> is null.</exception>
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

        /// <summary>
        /// Determines whether at least one value in the sequence is <see langword="true"/>.
        /// </summary>
        /// <param name="enumerable">The sequence to evaluate.</param>
        /// <returns><see langword="true"/> if any value is true; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> is null.</exception>
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
}
