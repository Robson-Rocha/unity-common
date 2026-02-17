using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Extension methods for Transform component
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Resets the transform's local position, rotation, and scale to defaults
        /// </summary>
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Sets the X component of the transform's local position
        /// </summary>
        public static void SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 position = transform.localPosition;
            position.x = x;
            transform.localPosition = position;
        }

        /// <summary>
        /// Sets the Y component of the transform's local position
        /// </summary>
        public static void SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 position = transform.localPosition;
            position.y = y;
            transform.localPosition = position;
        }

        /// <summary>
        /// Sets the Z component of the transform's local position
        /// </summary>
        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 position = transform.localPosition;
            position.z = z;
            transform.localPosition = position;
        }
    }
}
