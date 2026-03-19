using UnityEngine;
using UnityEngine.SceneManagement;

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

        protected override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += HandleSceneLoaded;
            SceneManager.sceneUnloaded += HandleSceneUnloaded;
        }

        protected void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneUnloaded -= HandleSceneUnloaded;
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
        /// Immediately refreshes the <see cref="Targets"/> cache by scanning the scene for all <see cref="Detectable"/> objects.
        /// </summary>
        public void RefreshTargets()
        {
            Targets = FindObjectsByType<Detectable>(FindObjectsSortMode.None);
            _refreshTimer = RefreshIntervalInSeconds;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshTargets();
        }

        private void HandleSceneUnloaded(Scene scene)
        {
            Targets = System.Array.Empty<Detectable>();
        }
    }
}
