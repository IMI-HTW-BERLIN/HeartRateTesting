using System;
using GameObjects.UserInteraction;
using UnityEngine;

namespace GameObjects
{
    public class Door : Activatable
    {
        [Header("Scaling")] [SerializeField] private Vector3 startScale;
        [SerializeField] private Vector3 endScale;
        [SerializeField] private float scaleAnimationTime;

        [Header("Movement")] [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private float moveAnimationTime;


        private float _animationTime;

        private Vector3 _scaleOffset;

        private void Awake()
        {
            _scaleOffset = 0.5f * (startScale - endScale);
        }

        private void Update()
        {
            float delta;
            if (_animationTime <= 1)
            {
                // Scale
                transform.localScale = Vector3.Lerp(startScale, endScale, _animationTime);
                //Apply scale offset
                transform.position = Vector3.Lerp(startPosition, startPosition - _scaleOffset, _animationTime);
                delta = Time.deltaTime / scaleAnimationTime;
            }
            else
            {
                // Move after scaling
                transform.position = Vector3.Lerp(startPosition - _scaleOffset, endPosition, _animationTime - 1);
                delta = Time.deltaTime / moveAnimationTime;
            }

            _animationTime += Activated ? delta : -delta;
            _animationTime = Mathf.Clamp(_animationTime, 0, 2);
        }

        protected override void OnActivation()
        {
        }

        protected override void OnDeactivation()
        {
        }
    }
}