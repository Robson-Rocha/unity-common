using System;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Detects and tracks the nearest tagged target within a directional field of view cone.
    /// </summary>
    [DefaultExecutionOrder(-20)]
    public class FieldOfViewDetector : DetectorBase
    {
        /// <summary>
        /// Field of view angle in degrees. A value of 90 means 45 degrees on each side of the facing direction.
        /// </summary>
        [Range(0f, 360f)]
        public float FieldOfViewAngle = 90f;

        /// <summary>
        /// Delegate invoked to get the current facing direction. Must be set for the detector to work and should return a normalized vector.
        /// </summary>
        public Func<Vector2> GetFacingDirection;

        private float _cachedFieldOfViewAngle = -1f;
        private float _cachedHalfAngleCos;

        protected override void PerformDetection()
        {
            if (GetFacingDirection == null)
                return;

            Vector2 facingDirection = GetFacingDirection();
            if (facingDirection == Vector2.zero)
                return;

            if (_cachedFieldOfViewAngle != FieldOfViewAngle)
            {
                _cachedFieldOfViewAngle = FieldOfViewAngle;
                _cachedHalfAngleCos = Mathf.Cos(FieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
            }

            float nearestDistance = float.MaxValue;
            Detectable nearestTarget = null;

            Vector2 detectionOrigin = GetDetectionOrigin();

            for (int i = 0; i < _targets.Length; i++)
            {
                Detectable t = _targets[i];
                
                if (t == null || t.gameObject == null || t.Undetectable || 
                    !MatchesNameFilter(t) || !IsInLayerMask(t.gameObject.layer))
                    continue;

                Transform detectionTransform = GetDetectionTransform(t);
                Vector2 directionToTarget = (Vector2)detectionTransform.position - detectionOrigin;
                float distance = directionToTarget.magnitude;

                if (distance > DetectionRadius)
                    continue;

                directionToTarget /= distance;
                float dotProduct = Vector2.Dot(facingDirection, directionToTarget);

                if (dotProduct < _cachedHalfAngleCos)
                    continue;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = t;
                }
            }

            if (nearestTarget != null && nearestTarget.gameObject != null)
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

            if (GetFacingDirection == null)
            {
                Gizmos.color = Color.yellow;
                DrawFieldOfViewCone(start, Vector2.up);
                DrawFieldOfViewCone(start, Vector2.down);
                DrawFieldOfViewCone(start, Vector2.left);
                DrawFieldOfViewCone(start, Vector2.right);
                return;
            }

            Vector2 facingDirection = GetFacingDirection();
            if (facingDirection == Vector2.zero)
                return;

            Gizmos.color = Color.green;
            DrawFieldOfViewCone(start, facingDirection);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, start + (Vector3)(facingDirection * DetectionRadius));

            if (IsDetected && DirectionToTarget.HasValue)
            {
                Gizmos.color = Color.red;
                Vector3 end = start + (Vector3)(DirectionToTarget.Value * DistanceToTarget.Value);

                Gizmos.DrawLine(start, end);

                float boxSize = 0.2f;
                Gizmos.DrawWireCube(end, new Vector3(boxSize, boxSize, boxSize));
            }
        }

        private void DrawFieldOfViewCone(Vector3 start, Vector2 facingDirection)
        {
            float halfAngle = FieldOfViewAngle * 0.5f;
            int segments = 20;

            Vector3 previousPoint = start;
            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = -halfAngle + (FieldOfViewAngle * i / segments);
                Vector2 direction = RotateVector(facingDirection, currentAngle);
                Vector3 point = start + (Vector3)(direction * DetectionRadius);

                if (i > 0)
                {
                    Gizmos.DrawLine(previousPoint, point);
                }

                previousPoint = point;
            }

            Vector2 leftBoundary = RotateVector(facingDirection, -halfAngle);
            Vector2 rightBoundary = RotateVector(facingDirection, halfAngle);

            Gizmos.DrawLine(start, start + (Vector3)(leftBoundary * DetectionRadius));
            Gizmos.DrawLine(start, start + (Vector3)(rightBoundary * DetectionRadius));
        }

        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector2(
                cos * v.x - sin * v.y,
                sin * v.x + cos * v.y
            );
        }
#endif
    }
}
