using UnityEngine;

namespace GameObjects.UserInteraction
{
    public abstract class Interactable : TimeScaleObject
    {
        [SerializeField] protected Activatable activatable;

        [ExecuteAlways]
        public void Interact()
        {
            OnInteract();
        }

        protected abstract void OnInteract();
    }
}