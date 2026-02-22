using System.Linq;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Detects and tracks the nearest tagged target around an origin transform within a configurable radius.
    /// </summary>
    [DefaultExecutionOrder(-20)]
    public class NearestDetector : MonoBehaviour
    {
        /// <summary>
        /// Origin transform used as the detection reference point. Defaults to this transform when null.
        /// </summary>
        public Transform Origin;

        /// <summary>
        /// Optional detector identifier.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// Detectable name used to find candidate targets. If null or empty, all Detectable objects are considered as potential targets regardless of their name.
        /// </summary>
        public string TargetDetectableName;

        /// <summary>
        /// Maximum distance from origin to consider a target detected.
        /// </summary>
        public float DetectionRadius = 5f;

        /// <summary>
        /// Optional offset from the origin position for detection calculations. This allows the detector to be centered around a different point than the origin transform, if needed.
        /// </summary>
        public Vector2 DetectionOffset = Vector2.zero; // Optional offset from the origin position for detection calculations

        /// <summary>
        /// Interval in seconds used to refresh the cached target list.
        /// </summary>
        public float RefreshTargetsIntervalInSeconds = 1f; // seconds

        /// <summary>
        /// Layer mask to filter which layers can be detected. Use -1 (Everything) to include all layers.
        /// </summary>
        public LayerMask DetectionLayerMask = -1; // Everything by default

        /// <summary>
        /// Gets whether a target is currently detected within the radius.
        /// </summary>
        public bool IsDetected { get; private set; }

        /// <summary>
        /// Gets the currently detected nearest target, or null when no target is detected.
        /// </summary>
        public GameObject Target { get; private set; }

        /// <summary>
        /// Gets the normalized direction from origin to the detected target, when available.
        /// </summary>
        public Vector2? DirectionToTarget { get; private set; } // Direction from Origin to detected target

        /// <summary>
        /// Gets the distance from origin to the detected target, when available.
        /// </summary>
        public float? DistanceToTarget { get; private set; } // Distance from Origin to detected target

        private Detectable[] _targets; // Caches potential targets with the configured tag.
        private float _refreshTimer; // Tracks time remaining until the next target refresh.

        // Initializes the origin reference and loads initial targets.
        void Start()
        {
            if (Origin == null)
            {
                Origin = transform;
            }
            RefreshTargets();
        }

        // Refreshes cached targets and resets the refresh timer.
        private void RefreshTargets()
        {
            _targets = GameObject.FindObjectsByType<Detectable>(FindObjectsSortMode.None);
            _refreshTimer = RefreshTargetsIntervalInSeconds;
        }

        // Finds and tracks the nearest valid target within detection range.
        void Update()
        {
            IsDetected = false;
            DirectionToTarget = null;
            DistanceToTarget = null;
            Target = null;

            _refreshTimer = _refreshTimer.DecrementTimer();
            if (_refreshTimer.IsNearZero())
            {
                RefreshTargets();
            }

            if (Origin == null || _targets == null || _targets.Length == 0)
                return;

            var nearest = _targets
                .Where(t => 
                    t != null &&
                    t.gameObject != null &&
                    t.Undetectable == false &&
                    MatchesNameFilter(t) && 
                    IsInLayerMask(t.gameObject.layer))
                .Select(t => new
                {
                    Target = t,
                    Distance = Vector2.Distance((Vector2)Origin.position + DetectionOffset, (Vector2)t.transform.position)
                })
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            if (nearest != null && nearest.Target.gameObject != null && nearest.Distance <= DetectionRadius)
            {
                IsDetected = true;
                DirectionToTarget = ((Vector2)nearest.Target.transform.position - (Vector2)Origin.position).normalized;
                DistanceToTarget = nearest.Distance;
                Target = nearest.Target.gameObject;
            }
        }

        private bool MatchesNameFilter(Detectable detectable)
        {
            // Empty filter matches all detectables
            if (string.IsNullOrWhiteSpace(TargetDetectableName))
                return true;

            // Otherwise, match the specific tag
            return detectable.Names.Contains(TargetDetectableName);
        }

        // Checks if a layer is included in the detection layer mask.
        private bool IsInLayerMask(int layer)
        {
            return (DetectionLayerMask.value & (1 << layer)) != 0;
        }

#if UNITY_EDITOR
        // Draws editor gizmos for detection radius and target direction.
        private void OnDrawGizmosSelected()
        {
            Vector3 start = Origin == null ? transform.position : Origin.position;
            start += (Vector3)DetectionOffset;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(start, DetectionRadius);

            if (IsDetected && DirectionToTarget.HasValue)
            {
                Gizmos.color = Color.red;
                Vector3 end = start + (Vector3)(DirectionToTarget.Value * DetectionRadius);

                Gizmos.DrawLine(start, end);

                float boxSize = 0.2f;
                Gizmos.DrawWireCube(end, new Vector3(boxSize, boxSize, boxSize));
            }
        }
#endif
    }
}