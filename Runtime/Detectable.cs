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
        /// Gets or sets the detectable names associated with the object.
        /// This name is used by NearestDetector components to identify and filter detectable objects.
        /// </summary>
        public List<string> Names = new();

        /// <summary>
        /// Gets or sets whether this object should be ignored by NearestDetector components, even if it matches the specified name and layer criteria.
        /// </summary>
        public bool Undetectable;
    }
}
