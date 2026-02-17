using UnityEngine;

public static class FloatExtensions
{
    public const float DEFAULT_TOLERANCE = 0.0001f;

    public static bool IsNearZero(this float value, float tolerance = DEFAULT_TOLERANCE) =>
        Mathf.Abs(value) < tolerance;

    public static bool IsBelowNearZero(this float value, float tolerance = DEFAULT_TOLERANCE) =>
        value < -tolerance;

    public static bool IsAboveNearZero(this float value, float tolerance = DEFAULT_TOLERANCE) =>
        value > tolerance;

    public static float DecrementTimer(this float timer, float? deltaTime = null) =>
        Mathf.Max(timer - (deltaTime ?? Time.deltaTime), 0f);
}