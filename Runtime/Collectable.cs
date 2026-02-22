using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    public abstract class Collectable : MonoBehaviour
    {
        public virtual bool IsCollected { get; protected set; } = false;
        public abstract void Collect(GameObject collector);
    }
}