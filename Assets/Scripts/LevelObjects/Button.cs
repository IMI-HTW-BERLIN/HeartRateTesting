using Animation;
using LevelObjects.UserInteraction;
using UnityEngine;

namespace LevelObjects
{
    public class Button : Pressable
    {
        [SerializeField] private Activatable activatable;
        [SerializeField] private CustomAnimator animator;

        protected override void OnPress()
        {
            activatable.Toggle();
            animator.ToggleAnimation();
        }
    }
}