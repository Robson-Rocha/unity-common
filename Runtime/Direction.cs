using System;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Defines the possible movement directions, allowing for combinations of cardinal and diagonal directions.
    /// </summary>
    [Flags] // Allows bitwise combinations
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right
    }

    /// <summary>
    /// Provides extension methods for the Direction enumeration, enabling convenient manipulation and querying of
    /// directional values.
    /// </summary>
    /// <remarks>These methods allow developers to check specific directional flags, flip directions
    /// horizontally or vertically, convert directions to angles, and transform directions to Vector2 representations.
    /// The extensions facilitate working with directional data in a more intuitive and readable manner, especially in
    /// scenarios involving movement, orientation, or physics calculations.</remarks>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Determines whether the specified direction includes the Up flag.
        /// </summary>
        /// <remarks>This method is an extension for the Direction type and uses the HasFlag method to
        /// evaluate whether the Up flag is set. It is useful for quickly checking directional states in scenarios such
        /// as movement or orientation logic.</remarks>
        /// <param name="direction">The direction value to check for the presence of the Up flag. This parameter cannot be null.</param>
        /// <returns>true if the direction includes the Up flag; otherwise, false.</returns>
        public static bool IsUp(this Direction direction)
            => direction.HasFlag(Direction.Up);

        /// <summary>
        /// Determines whether the specified direction includes the Down flag.
        /// </summary>
        /// <remarks>Use this method to test if a Direction value represents or contains the Down flag.
        /// This is useful when working with composite direction values in scenarios such as movement or navigation
        /// logic.</remarks>
        /// <param name="direction">The direction value to check for the presence of the Down flag. This parameter cannot be null.</param>
        /// <returns>true if the direction includes the Down flag; otherwise, false.</returns>
        public static bool IsDown(this Direction direction)
            => direction.HasFlag(Direction.Down);

        public static bool IsLeft(this Direction direction)
            => direction.HasFlag(Direction.Left);

        public static bool IsRight(this Direction direction)
            => direction.HasFlag(Direction.Right);

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

        public static Direction ToIncomingDirection(this Vector2 velocity)
        {
            Direction dir = Direction.None;
            if (velocity.y.IsBelowNearZero()) dir |= Direction.Up;
            if (velocity.y.IsAboveNearZero()) dir |= Direction.Down;
            if (velocity.x.IsBelowNearZero()) dir |= Direction.Right;
            if (velocity.x.IsAboveNearZero()) dir |= Direction.Left;
            return dir;
        }

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
}