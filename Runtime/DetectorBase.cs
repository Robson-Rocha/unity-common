using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Base class for components that detect and track detectable targets.
    /// </summary>
    public abstract class DetectorBase : MonoBehaviour
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
        public Vector2 DetectionOffset = Vector2.zero;

        /// <summary>
        /// Layer mask to filter which layers can be detected. Use -1 (Everything) to include all layers.
        /// </summary>
        public LayerMask DetectionLayerMask = -1;

        /// <summary>
        /// Gets whether a target is currently detected within the radius.
        /// </summary>
        public bool IsDetected { get; protected set; }

        /// <summary>
        /// Gets the currently detected nearest target, or null when no target is detected.
        /// </summary>
        public GameObject Target { get; protected set; }

        /// <summary>
        /// Gets the normalized direction from origin to the detected target, when available.
        /// </summary>
        public Vector2? DirectionToTarget { get; protected set; }

        /// <summary>
        /// Gets the distance from origin to the detected target, when available.
        /// </summary>
        public float? DistanceToTarget { get; protected set; }

        protected Detectable[] Targets => DetectableManager.Instance != null
            ? DetectableManager.Instance.Targets
            : System.Array.Empty<Detectable>();

        protected virtual void Start()
        {
            if (Origin == null)
            {
                Origin = transform;
            }
        }

        protected virtual void Update()
        {
            IsDetected = false;
            DirectionToTarget = null;
            DistanceToTarget = null;
            Target = null;

            if (Origin == null || Targets == null || Targets.Length == 0)
                return;

            PerformDetection();
        }

        protected abstract void PerformDetection();

        protected bool MatchesNameFilter(Detectable detectable)
        {
            if (string.IsNullOrWhiteSpace(TargetDetectableName))
                return true;

            foreach (var detectionPoint in detectable.DetectionPoints)
            {
                if (detectionPoint.Name == TargetDetectableName)
                    return true;
            }

            return false;
        }

        protected Transform GetDetectionTransform(Detectable detectable)
        {
            if (string.IsNullOrWhiteSpace(TargetDetectableName))
                return detectable.transform;

            foreach (var detectionPoint in detectable.DetectionPoints)
            {
                if (detectionPoint.Name == TargetDetectableName)
                {
                    return detectionPoint.DetectionTransform != null ? detectionPoint.DetectionTransform : detectable.transform;
                }
            }

            return detectable.transform;
        }

        protected bool IsInLayerMask(int layer)
        {
            return (DetectionLayerMask.value & (1 << layer)) != 0;
        }

        protected Vector2 GetDetectionOrigin()
        {
            return (Vector2)Origin.position + DetectionOffset;
        }
    }
}
