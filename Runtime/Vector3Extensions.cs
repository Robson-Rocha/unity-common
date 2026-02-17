using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Extension methods for Vector3
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Returns a new Vector3 with the X component replaced
        /// </summary>
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        /// <summary>
        /// Returns a new Vector3 with the Y component replaced
        /// </summary>
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        /// <summary>
        /// Returns a new Vector3 with the Z component replaced
        /// </summary>
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        /// Returns a Vector2 with the X and Y components
        /// </summary>
        public static Vector2 ToVector2XY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        /// <summary>
        /// Returns a Vector2 with the X and Z components
        /// </summary>
        public static Vector2 ToVector2XZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        /// Returns the Vector3 with each component set to its absolute value
        /// </summary>
        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }
    }
}
