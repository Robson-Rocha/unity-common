using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Singleton manager that maintains a shared, periodically refreshed cache of all <see cref="Detectable"/>
    /// objects in the scene. Add this component to the scene so all <see cref="DetectorBase"/> instances
    /// share a single <see cref="UnityEngine.Object.FindObjectsByType{T}"/> call per refresh cycle.
    /// </summary>
    [DefaultExecutionOrder(-30)]
    public class DetectableManager : SingletonMonoBehaviour<DetectableManager>
    {
        protected override bool KeepInScene => true;

        /// <summary>
        /// Interval in seconds between full scene scans that rebuild the <see cref="Targets"/> cache.
        /// Set to zero to scan only once at startup.
        /// </summary>
        public float RefreshIntervalInSeconds = 1f;

        /// <summary>
        /// Gets the current cached array of all <see cref="Detectable"/> objects in the scene.
        /// </summary>
        public Detectable[] Targets { get; private set; } = System.Array.Empty<Detectable>();

        private float _refreshTimer;

        protected override void Awake()
        {
            if (!CanAwake())
                return;
            RefreshTargets();
        }

        private void Update()
        {
            if (!RefreshIntervalInSeconds.IsAboveNearZero())
                return;

            _refreshTimer.DecrementTimer();
            if (_refreshTimer.IsNearZero())
                RefreshTargets();
        }

        /// <summary>
        /// Forces an immediate rebuild of the <see cref="Targets"/> cache and resets the refresh timer.
        /// </summary>
        public void ForceRefresh() => RefreshTargets();

        private void RefreshTargets()
        {
            Targets = FindObjectsByType<Detectable>(FindObjectsSortMode.None);
            _refreshTimer = RefreshIntervalInSeconds;
        }
    }
}
