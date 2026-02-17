using UnityEngine;

/// <summary>
/// Provides helper extension methods for working with <see cref="Color"/> values.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Rounds each color channel to the specified number of decimal places.
    /// </summary>
    /// <param name="c">The source color.</param>
    /// <param name="decimals">The number of decimal places used for rounding.</param>
    /// <returns>A new color with rounded channel values.</returns>
    public static Color RoundColor(this Color c, float decimals = 3f)
    {
        return new Color(
            Mathf.Round(c.r * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals),
            Mathf.Round(c.g * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals),
            Mathf.Round(c.b * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals),
            Mathf.Round(c.a * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals)
        );
    }

    /// <summary>
    /// Creates a new color with the same RGB channels and a new alpha value.
    /// </summary>
    /// <param name="c">The source color.</param>
    /// <param name="a">The alpha value to set.</param>
    /// <returns>A color with updated alpha channel.</returns>
    public static Color SetAlpha(this Color c, float a) =>
        new(c.r, c.g, c.b, a);
}
