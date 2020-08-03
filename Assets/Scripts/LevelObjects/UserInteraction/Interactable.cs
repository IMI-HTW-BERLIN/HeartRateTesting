using UnityEngine;

namespace LevelObjects.UserInteraction
{
    /// <summary>
    /// E.g Pressure Plates
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected Activatable activatable;

        public void Interact()
        {
            OnInteract();
        }

        protected abstract void OnInteract();
    }
}