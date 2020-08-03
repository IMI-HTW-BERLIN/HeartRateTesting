using Animation;
using LevelObjects.UserInteraction;
using UnityEngine;

namespace LevelObjects
{
    public class PressurePlate : Interactable
    {
        [SerializeField] private CustomAnimator animator;

        protected override void OnInteract()
        {
            activatable.Toggle();
        }

        private void OnTriggerEnter(Collider other)
        {
            animator.AnimateForward();
            activatable.Activate();
        }

        private void OnTriggerExit(Collider other)
        {
            animator.AnimateBackwards();
            activatable.Deactivate();
        }
    }
}