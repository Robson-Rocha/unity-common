using UnityEngine;

public static class Rigidbody2DExtensions
{
    public static void ApplyVelocity(this Rigidbody2D rb, Vector2 xy, float? xMult = null, float? yMult = null) =>
        ApplyVelocity(rb, xy.x, xMult, xy.y, yMult);

    public static void ApplyVelocity(this Rigidbody2D rb, float? x = null, float? xMult = null, float? y = null, float? yMult = null)
    {
        rb.linearVelocity =
            new Vector2(
                (x ?? rb.linearVelocityX) * (xMult ?? 1f),
                (y ?? rb.linearVelocityY) * (yMult ?? 1f));
    }
}
