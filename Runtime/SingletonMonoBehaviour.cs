using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour
    where T : MonoBehaviour
    {
        protected virtual bool KeepInScene { get => false; }

        [HideInInspector]
        public static T Instance { get; protected set; }

        protected virtual bool CanAwake()
        {
            if (Instance != null && Instance != this as T)
            {
                Destroy(gameObject);
                return false;
            }
            Instance = this as T;
            if (!KeepInScene)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            return true;
        }

        protected virtual void Awake() => CanAwake();

        protected virtual void OnEnable()
        {
            if (Instance == null)
                Instance = this as T;
        }
    }
}