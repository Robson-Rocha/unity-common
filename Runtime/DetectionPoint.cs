using System;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    [Serializable]
    public class DetectionPoint : IEquatable<DetectionPoint>
    {
        public string Name;
        public Transform DetectionTransform; // Optional transform to be used as the reference point for detection instead of the GameObject's main transform

        public bool Equals(DetectionPoint other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && DetectionTransform == other.DetectionTransform;
        }

        public override bool Equals(object obj)
        {
            return obj is DetectionPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DetectionTransform);
        }
    }
}
