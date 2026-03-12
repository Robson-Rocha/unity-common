using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Detects and tracks the nearest tagged target around an origin transform within a configurable radius.
    /// </summary>
    [DefaultExecutionOrder(-20)]
    public class NearestDetector : DetectorBase
    {
        protected override void PerformDetection()
        {
            float nearestDistance = float.MaxValue;
            Detectable nearestTarget = null;

            Vector2 detectionOrigin = GetDetectionOrigin();

            for (int i = 0; i < Targets.Length; i++)
            {
                Detectable t = Targets[i];
                
                if (t == null || t.gameObject == null || t.Undetectable || 
                    !MatchesNameFilter(t) || !IsInLayerMask(t.gameObject.layer))
                    continue;

                Transform detectionTransform = GetDetectionTransform(t);
                float distance = Vector2.Distance(detectionOrigin, (Vector2)detectionTransform.position);
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = t;
                }
            }

            if (nearestTarget != null && nearestTarget.gameObject != null && nearestDistance <= DetectionRadius)
            {
                IsDetected = true;
                Transform detectionTransform = GetDetectionTransform(nearestTarget);
                DirectionToTarget = ((Vector2)detectionTransform.position - detectionOrigin).normalized;
                DistanceToTarget = nearestDistance;
                Target = nearestTarget.gameObject;
            }
        }

#if UNITY_EDITOR
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