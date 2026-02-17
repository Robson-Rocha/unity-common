using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Extension methods for GameObject
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets a component from the GameObject, adding it if it doesn't exist
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Destroys all child GameObjects
        /// </summary>
        public static void DestroyChildren(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Destroys all child GameObjects immediately (for use in editor)
        /// </summary>
        public static void DestroyChildrenImmediate(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Sets the layer of this GameObject and all its children recursively
        /// </summary>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }
    }
}
