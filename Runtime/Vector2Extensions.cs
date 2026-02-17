using UnityEngine;

public static class Vector2Extensions
{
    public static bool HasRight(this Vector2 vector) =>
        vector.x.IsAboveNearZero();

    public static bool HasLeft(this Vector2 vector) =>
        vector.x.IsBelowNearZero();

    public static bool HasHorizontal(this Vector2 vector) =>
        !vector.x.IsNearZero();

    public static bool HasUp(this Vector2 vector) =>
        vector.y.IsAboveNearZero();

    public static bool HasDown(this Vector2 vector) =>
        vector.y.IsBelowNearZero();

    public static bool HasVertical(this Vector2 vector) =>
        !vector.y.IsNearZero();

    public static bool IsNearEqualTo(this Vector3 vector, Vector2 other, float tolerance = FloatExtensions.DEFAULT_TOLERANCE) =>
        ((Vector2)vector).IsNearEqualTo(other, tolerance);

    public static bool IsNearEqualTo(this Vector2 vector, Vector2 other, float tolerance = FloatExtensions.DEFAULT_TOLERANCE) =>
        Mathf.Abs(vector.x - other.x) < tolerance && Mathf.Abs(vector.y - other.y) < tolerance;

    public static float GetAngle(this Vector2 vector) =>
        Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

    public static float GetNormalizedAngle(this Vector2 vector) =>
        (vector.GetAngle() + 360f) % 360f;
}