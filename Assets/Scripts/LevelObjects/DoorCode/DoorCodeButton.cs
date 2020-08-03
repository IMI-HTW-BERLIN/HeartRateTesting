using System;
using Animation;
using LevelObjects.UserInteraction;
using UnityEngine;
using UnityEngine.UI;

namespace LevelObjects.DoorCode
{
    public class DoorCodeButton : Pressable
    {
        [SerializeField] private int doorCodeNumber;
        [SerializeField] private CustomAnimator animator;
        [SerializeField] private Image imgSelected;


        public event Action<int> OnDoorCodeButtonPressed;

        protected override void OnPress()
        {
            OnDoorCodeButtonPressed?.Invoke(doorCodeNumber);
            animator.AnimateForward();
        }

        protected override void OnShowInReach()
        {
            imgSelected.gameObject.SetActive(true);
        }

        protected override void OnHideInReach()
        {
            imgSelected.gameObject.SetActive(false);
        }
    }
}