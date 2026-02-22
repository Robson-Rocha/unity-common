using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Provides helper methods for querying and comparing <see cref="Vector2"/> values.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Checks whether the vector has a positive horizontal component.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns><see langword="true"/> when X is positive beyond tolerance; otherwise <see langword="false"/>.</returns>
        public static bool HasRight(this Vector2 vector) =>
            vector.x.IsAboveNearZero();

        /// <summary>
        /// Checks whether the vector has a negative horizontal component.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns><see langword="true"/> when X is negative beyond tolerance; otherwise <see langword="false"/>.</returns>
        public static bool HasLeft(this Vector2 vector) =>
            vector.x.IsBelowNearZero();

        /// <summary>
        /// Checks whether the vector has a non-zero horizontal component.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns><see langword="true"/> when X is non-zero beyond tolerance; otherwise <see langword="false"/>.</returns>
        public static bool HasHorizontal(this Vector2 vector) =>
            !vector.x.IsNearZero();

        /// <summary>
        /// Checks whether the vector has a positive vertical component.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns><see langword="true"/> when Y is positive beyond tolerance; otherwise <see langword="false"/>.</returns>
        public static bool HasUp(this Vector2 vector) =>
            vector.y.IsAboveNearZero();

        /// <summary>
        /// Checks whether the vector has a negative vertical component.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns><see langword="true"/> when Y is negative beyond tolerance; otherwise <see langword="false"/>.</returns>
        public static bool HasDown(this Vector2 vector) =>
            vector.y.IsBelowNearZero();

        /// <summary>
        /// Checks whether the vector has a non-zero vertical component.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns><see langword="true"/> when Y is non-zero beyond tolerance; otherwise <see langword="false"/>.</returns>
        public static bool HasVertical(this Vector2 vector) =>
            !vector.y.IsNearZero();

        /// <summary>
        /// Compares a <see cref="Vector3"/> against a <see cref="Vector2"/> using tolerance.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="other">The comparison vector.</param>
        /// <param name="tolerance">Maximum absolute delta allowed per axis.</param>
        /// <returns><see langword="true"/> when both axes are within tolerance; otherwise <see langword="false"/>.</returns>
        public static bool IsNearEqualTo(this Vector3 vector, Vector2 other, float tolerance = FloatExtensions.DEFAULT_TOLERANCE) =>
            ((Vector2)vector).IsNearEqualTo(other, tolerance);

        /// <summary>
        /// Compares two <see cref="Vector2"/> values using tolerance.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="other">The comparison vector.</param>
        /// <param name="tolerance">Maximum absolute delta allowed per axis.</param>
        /// <returns><see langword="true"/> when both axes are within tolerance; otherwise <see langword="false"/>.</returns>
        public static bool IsNearEqualTo(this Vector2 vector, Vector2 other, float tolerance = FloatExtensions.DEFAULT_TOLERANCE) =>
            Mathf.Abs(vector.x - other.x) < tolerance && Mathf.Abs(vector.y - other.y) < tolerance;

        /// <summary>
        /// Determines whether the specified vector is approximately equal to the zero vector within a given tolerance.
        /// </summary>
        /// <remarks>This method is useful for scenarios where floating-point precision may lead to inaccuracies,
        /// allowing for a more flexible comparison.</remarks>
        /// <param name="vector">The vector to evaluate for proximity to the zero vector.</param>
        /// <param name="tolerance">The maximum allowable difference between the vector and the zero vector for the comparison to be considered
        /// true. Must be a non-negative value.</param>
        /// <returns>true if the vector is within the specified tolerance of the zero vector; otherwise, false.</returns>
        public static bool IsNearZero(this Vector2 vector, float tolerance = FloatExtensions.DEFAULT_TOLERANCE) =>
            vector.IsNearEqualTo(Vector2.zero, tolerance);

        /// <summary>
        /// Gets the vector angle in degrees using <c>atan2</c>.
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns>The angle in degrees in the range [-180, 180].</returns>
        public static float GetAngle(this Vector2 vector) =>
            Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        /// <summary>
        /// Gets the vector angle normalized to the range [0, 360).
        /// </summary>
        /// <param name="vector">The vector to evaluate.</param>
        /// <returns>The normalized angle in degrees.</returns>
        public static float GetNormalizedAngle(this Vector2 vector) =>
            (vector.GetAngle() + 360f) % 360f;

        /// <summary>
        /// Snaps the specified direction vector to the nearest multiple of the given angle increment, returning a normalized vector representing the snapped direction.
        /// </summary>
        /// <param name="direction">The direction vector to be snapped. If this vector is <see cref="Vector2.zero"/>, a default downward direction is returned.</param>
        /// <param name="angleIncrement">The angle increment, in degrees, to which the direction will be snapped. Must be greater than zero.</param>
        /// <returns>A normalized <see cref="Vector2"/> representing the direction snapped to the nearest specified angle increment. If the input direction is zero, returns <see cref="Vector2.down"/>.</returns>
        public static Vector2 SnapToAngle(this Vector2 direction, float angleIncrement)
        {
            if (direction == Vector2.zero)
                return Vector2.down; // Default fallback

            // Get the angle in degrees (0 = right, counter-clockwise)
            float angle = direction.GetAngle();

            // Snap to nearest increment
            float snappedAngle = Mathf.Round(angle / angleIncrement) * angleIncrement;

            // Convert back to radians and then to direction vector
            float radians = snappedAngle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }
    }
}
