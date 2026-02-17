using UnityEngine;

/// <summary>
/// Provides helper methods for applying 2D rigidbody velocity.
/// </summary>
public static class Rigidbody2DExtensions
{
    /// <summary>
    /// Applies velocity values from a vector, with optional axis multipliers.
    /// </summary>
    /// <param name="rb">The rigidbody to update.</param>
    /// <param name="xy">The velocity values for X and Y.</param>
    /// <param name="xMult">Optional multiplier applied to X velocity.</param>
    /// <param name="yMult">Optional multiplier applied to Y velocity.</param>
    public static void ApplyVelocity(this Rigidbody2D rb, Vector2 xy, float? xMult = null, float? yMult = null) =>
        ApplyVelocity(rb, xy.x, xMult, xy.y, yMult);

    /// <summary>
    /// Applies velocity values with optional per-axis value overrides and multipliers.
    /// </summary>
    /// <param name="rb">The rigidbody to update.</param>
    /// <param name="x">Optional X value. Uses current X velocity when null.</param>
    /// <param name="xMult">Optional multiplier applied to resulting X velocity.</param>
    /// <param name="y">Optional Y value. Uses current Y velocity when null.</param>
    /// <param name="yMult">Optional multiplier applied to resulting Y velocity.</param>
    public static void ApplyVelocity(this Rigidbody2D rb, float? x = null, float? xMult = null, float? y = null, float? yMult = null)
    {
        rb.linearVelocity =
            new Vector2(
                (x ?? rb.linearVelocityX) * (xMult ?? 1f),
                (y ?? rb.linearVelocityY) * (yMult ?? 1f));
    }
}
