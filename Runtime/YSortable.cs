using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Automatically sets the SpriteRenderer's sorting order based on the GameObject's Y position to achieve a simple 2D depth effect.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class YSortable : MonoBehaviour
    {
        [Tooltip("Offset to adjust the Y position used for sorting. Positive values move the sort point up.")]
        [SerializeField] private float _yOffset = 0f;

        // Reference to the SpriteRenderer component on this GameObject
        private SpriteRenderer sr;

        // Awake is called when the script instance is being loaded
        void Awake() 
        { 
            this.TryInitComponent(ref sr);
        }

        // LateUpdate is called every frame, after all Update functions have been called
        void LateUpdate() 
        {
            // Set the sorting order based on the Y position (with offset), multiplying by 100 to allow for finer granularity
            sr.sortingOrder = Mathf.RoundToInt(-(transform.position.y + _yOffset) * 100); 
        }
    }
}