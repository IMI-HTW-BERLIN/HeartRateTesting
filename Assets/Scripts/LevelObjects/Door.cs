using Animation;
using LevelObjects.UserInteraction;
using UnityEngine;

namespace LevelObjects
{
    public class Door : Activatable
    {
        [SerializeField] private CustomAnimator animator;

        protected override void OnActivation()
        {
            animator.AnimateForward();
        }

        protected override void OnDeactivation()
        {
            animator.AnimateBackwards();
        }
    }
}