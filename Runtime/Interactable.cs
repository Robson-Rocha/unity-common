using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    [RequireComponent(typeof(Detectable))]
    public abstract class Interactable : MonoBehaviour
    {
        protected Detectable detectable;

        protected virtual void Awake()
        {
            if (this.TryInitComponent(ref detectable))
            {
                detectable.Names.AddIfNotExists(nameof(Interactable));
            }
        }

        public virtual string InteractionVerb { get; } = "Interact";

        public virtual Vector3 InteractionOffset { get; } = default;

        public abstract void Interact(GameObject interactor, Vector3 interactorTransformPosition);
    }
}