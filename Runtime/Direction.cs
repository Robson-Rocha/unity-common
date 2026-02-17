using System;
using UnityEngine;

[Flags] // Allows bitwise combinations
/// <summary>
/// Represents cardinal and diagonal directions that can be combined with bit flags.
/// </summary>
public enum Direction
{
    /// <summary>
    /// No direction.
    /// </summary>
    None = 0,
    /// <summary>
    /// Up direction.
    /// </summary>
    Up = 1,
    /// <summary>
    /// Down direction.
    /// </summary>
    Down = 2,
    /// <summary>
    /// Left direction.
    /// </summary>
    Left = 4,
    /// <summary>
    /// Right direction.
    /// </summary>
    Right = 8,
    /// <summary>
    /// Up-left diagonal direction.
    /// </summary>
    UpLeft = Up | Left,
    /// <summary>
    /// Up-right diagonal direction.
    /// </summary>
    UpRight = Up | Right,
    /// <summary>
    /// Down-left diagonal direction.
    /// </summary>
    DownLeft = Down | Left,
    /// <summary>
    /// Down-right diagonal direction.
    /// </summary>
    DownRight = Down | Right
}

/// <summary>
/// Provides helper methods for converting and transforming <see cref="Direction"/> values.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Checks whether the direction includes an up component.
    /// </summary>
    /// <param name="direction">The direction to inspect.</param>
    /// <returns><see langword="true"/> if it includes up; otherwise <see langword="false"/>.</returns>
    public static bool IsUp(this Direction direction)
        => direction.HasFlag(Direction.Up);

    /// <summary>
    /// Checks whether the direction includes a down component.
    /// </summary>
    /// <param name="direction">The direction to inspect.</param>
    /// <returns><see langword="true"/> if it includes down; otherwise <see langword="false"/>.</returns>
    public static bool IsDown(this Direction direction)
        => direction.HasFlag(Direction.Down);

    /// <summary>
    /// Checks whether the direction includes a left component.
    /// </summary>
    /// <param name="direction">The direction to inspect.</param>
    /// <returns><see langword="true"/> if it includes left; otherwise <see langword="false"/>.</returns>
    public static bool IsLeft(this Direction direction)
        => direction.HasFlag(Direction.Left);

    /// <summary>
    /// Checks whether the direction includes a right component.
    /// </summary>
    /// <param name="direction">The direction to inspect.</param>
    /// <returns><see langword="true"/> if it includes right; otherwise <see langword="false"/>.</returns>
    public static bool IsRight(this Direction direction)
        => direction.HasFlag(Direction.Right);

    /// <summary>
    /// Flips left and right components while preserving vertical components.
    /// </summary>
    /// <param name="direction">The direction to flip.</param>
    /// <returns>The horizontally flipped direction.</returns>
    public static Direction FlipHorizontally(this Direction direction)
    {
        Direction flippedDirection = direction;
        if (flippedDirection.IsLeft())
        {
            flippedDirection &= ~Direction.Left;
            flippedDirection |= Direction.Right;
        }
        else if (flippedDirection.IsRight())
        {
            flippedDirection &= ~Direction.Right;
            flippedDirection |= Direction.Left;
        }
        return flippedDirection;
    }

    /// <summary>
    /// Flips up and down components while preserving horizontal components.
    /// </summary>
    /// <param name="direction">The direction to flip.</param>
    /// <returns>The vertically flipped direction.</returns>
    public static Direction FlipVertically(this Direction direction)
    {
        Direction flippedDirection = direction;
        if (flippedDirection.IsUp())
        {
            flippedDirection &= ~Direction.Up;
            flippedDirection |= Direction.Down;
        }
        else if (flippedDirection.IsDown())
        {
            flippedDirection &= ~Direction.Down;
            flippedDirection |= Direction.Up;
        }
        return flippedDirection;
    }

    /// <summary>
    /// Converts a direction to an angle in degrees.
    /// </summary>
    /// <param name="direction">The direction to convert.</param>
    /// <returns>The matching angle in degrees, or <see langword="null"/> for unsupported values.</returns>
    public static float? ToAngle(this Direction direction)
        => direction switch
        {
            Direction.Right => 0f,
            Direction.Up | Direction.Right => 45f,
            Direction.Up => 90f,
            Direction.Up | Direction.Left => 135f,
            Direction.Left => 180f,
            Direction.Down | Direction.Left => 225f,
            Direction.Down => 270f,
            Direction.Down | Direction.Right => 315f,
            _ => null // fallback to null if no direction matched
        };

    /// <summary>
    /// Converts a direction to a normalized <see cref="Vector2"/>.
    /// </summary>
    /// <param name="direction">The direction to convert.</param>
    /// <returns>A unit vector representing the direction, or <see cref="Vector2.zero"/> when none is set.</returns>
    public static Vector2 ToVector2(this Direction direction)
        => direction switch
        {
            Direction.Right => Vector2.right,
            Direction.Up | Direction.Right => (Vector2.up + Vector2.right).normalized,
            Direction.Up => Vector2.up,
            Direction.Up | Direction.Left => (Vector2.up + Vector2.left).normalized,
            Direction.Left => Vector2.left,
            Direction.Down | Direction.Left => (Vector2.down + Vector2.left).normalized,
            Direction.Down => Vector2.down,
            Direction.Down | Direction.Right => (Vector2.down + Vector2.right).normalized,
            _ => Vector2.zero // fallback for None or unrecognized combinations
        };

    /// <summary>
    /// Converts velocity to the incoming direction relative to an impacted object.
    /// </summary>
    /// <param name="velocity">The velocity vector to evaluate.</param>
    /// <returns>The incoming direction derived from velocity signs.</returns>
    public static Direction ToIncomingDirection(this Vector2 velocity)
    {
        Direction dir = Direction.None;
        if (velocity.y.IsBelowNearZero()) dir |= Direction.Up;
        if (velocity.y.IsAboveNearZero()) dir |= Direction.Down;
        if (velocity.x.IsBelowNearZero()) dir |= Direction.Right;
        if (velocity.x.IsAboveNearZero()) dir |= Direction.Left;
        return dir;
    }

    /// <summary>
    /// Converts velocity to its movement direction.
    /// </summary>
    /// <param name="velocity">The velocity vector to evaluate.</param>
    /// <returns>The movement direction derived from velocity signs.</returns>
    public static Direction ToDirection(this Vector2 velocity)
    {
        Direction dir = Direction.None;
        if (velocity.y.IsBelowNearZero()) dir |= Direction.Down;
        if (velocity.y.IsAboveNearZero()) dir |= Direction.Up;
        if (velocity.x.IsBelowNearZero()) dir |= Direction.Left;
        if (velocity.x.IsAboveNearZero()) dir |= Direction.Right;
        return dir;
    }
}
