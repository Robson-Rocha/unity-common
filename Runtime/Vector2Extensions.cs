using UnityEngine;

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
}