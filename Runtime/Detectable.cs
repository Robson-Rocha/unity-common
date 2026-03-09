using System.Collections.Generic;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Marks a GameObject as detectable by NearestDetector components using the specified name.
    /// </summary>
    public class Detectable : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the collection of detection points used for analysis.
        /// </summary>
        /// <remarks>The list can be modified to add or remove detection points as needed. Changes to this
        /// collection affect which points are considered during the detection process.</remarks>
        public List<DetectionPoint> DetectionPoints = new();

        /// <summary>
        /// Gets or sets whether this object should be ignored by NearestDetector components, even if it matches the specified name and layer criteria.
        /// </summary>
        public bool Undetectable;
    }
}
