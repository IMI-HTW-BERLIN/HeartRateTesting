using GameObjects.UserInteraction;
using UnityEngine;

namespace GameObjects
{
    public class PressurePlate : Interactable
    {
        [Header("Scaling")] [SerializeField] private GameObject toBeScaled;
        [SerializeField] private Vector3 startScale;
        [SerializeField] private Vector3 endScale;
        [SerializeField] private float scaleAnimationTime;

        private float _animationTime;
        private bool _isBeingPressed;

        protected override void TimeScaledUpdate(float timeScaledDeltaTime)
        {
            toBeScaled.transform.localScale = Vector3.Lerp(startScale, endScale, _animationTime);
            _animationTime += (_isBeingPressed ? 1 : -1) / scaleAnimationTime * timeScaledDeltaTime;
            _animationTime = Mathf.Clamp(_animationTime, 0, 1);
        }

        protected override void TimeScaledFixedUpdate(float timeScaledFixedDeltaTime)
        {
        }

        protected override void OnInteract()
        {
            activatable.Toggle();
        }

        private void OnTriggerEnter(Collider other)
        {
            _isBeingPressed = true;
            activatable.Activate();
        }

        private void OnTriggerExit(Collider other)
        {
            _isBeingPressed = false;
            activatable.Deactivate();
        }
    }
}