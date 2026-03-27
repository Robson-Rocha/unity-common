using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Provides helper methods for float comparisons and timer operations.
    /// </summary>
    public static class FloatExtensions
    {
        /// <summary>
        /// Default tolerance used for near-zero comparisons.
        /// </summary>
        public const float DEFAULT_TOLERANCE = 0.0001f;

        /// <summary>
        /// Checks whether a value is close to zero within a tolerance.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="tolerance">The allowed absolute distance from zero.</param>
        /// <returns><see langword="true"/> when the value is near zero; otherwise <see langword="false"/>.</returns>
        public static bool IsNearZero(this float value, float tolerance = DEFAULT_TOLERANCE) =>
            Mathf.Abs(value) < tolerance;

        /// <summary>
        /// Checks whether a value is significantly below zero.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="tolerance">The near-zero tolerance threshold.</param>
        /// <returns><see langword="true"/> when the value is below negative tolerance; otherwise <see langword="false"/>.</returns>
        public static bool IsBelowNearZero(this float value, float tolerance = DEFAULT_TOLERANCE) =>
            value < -tolerance;

        /// <summary>
        /// Checks whether a value is significantly above zero.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="tolerance">The near-zero tolerance threshold.</param>
        /// <returns><see langword="true"/> when the value is above tolerance; otherwise <see langword="false"/>.</returns>
        public static bool IsAboveNearZero(this float value, float tolerance = DEFAULT_TOLERANCE) =>
            value > tolerance;

        /// <summary>
        /// Decrements a timer and clamps the result to zero.
        /// </summary>
        /// <param name="timer">The timer value to decrement.</param>
        /// <param name="deltaTime">Optional custom delta time. Uses <see cref="Time.deltaTime"/> when null.</param>
        /// <returns>The updated timer value after decrementing.</returns>
        public static float DecrementTimer(this ref float timer, float? deltaTime = null)
        {
            if (timer != 0f)
            {
                timer = Mathf.Max(timer - (deltaTime ?? Time.deltaTime), 0f);
            }
            return timer;
        }

        /// <summary>
        /// Rolls a random value to determine if an event should occur based on the provided chance.
        /// </summary>
        /// <param name="chance">The chance value to evaluate.</param>
        /// <param name="maxChance">The maximum value for the random roll.</param>
        /// <returns><see langword="true"/> if the event occurs; otherwise <see langword="false"/>.</returns>
        public static bool RollChance(this float chance, float maxChance = 1f) =>
            chance.IsAboveNearZero() && Random.Range(0f, maxChance) < chance;
    }
}
