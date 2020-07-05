using UnityEngine;

namespace GameObjects.UserInteraction
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private Activatable activatable;


        public abstract void Interact();

    }
}