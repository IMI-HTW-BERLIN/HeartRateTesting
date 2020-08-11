using System;
using Animation;
using LevelObjects.UserInteraction;
using UnityEngine;

namespace LevelObjects.DoorCode
{
    public class DoorCodeButton : Pressable
    {
        [SerializeField] private int doorCodeNumber;
        [SerializeField] private CustomAnimator animator;


        public event Action<int> OnDoorCodeButtonPressed;

        protected override void OnPress()
        {
            OnDoorCodeButtonPressed?.Invoke(doorCodeNumber);
            animator.AnimateForward();
        }
    }
}