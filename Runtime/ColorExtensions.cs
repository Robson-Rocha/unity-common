using UnityEngine;

public static class ColorExtensions
{
    public static Color RoundColor(this Color c, float decimals = 3f)
    {
        return new Color(
            Mathf.Round(c.r * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals),
            Mathf.Round(c.g * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals),
            Mathf.Round(c.b * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals),
            Mathf.Round(c.a * Mathf.Pow(10f, decimals)) / Mathf.Pow(10f, decimals)
        );
    }

    public static Color SetAlpha(this Color c, float a) =>
        new(c.r, c.g, c.b, a);
}
