using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public class CustomAnimator : TimeScaleObject
    {
        [SerializeField] private GameObject objectToAnimate;
        [SerializeField] private bool pingPongBack;
        [SerializeField] private bool loop;
        [SerializeField] private bool playOnAwake;
        [SerializeField] private bool useRigidbody;
        [SerializeField] private List<AnimationData> animationData;

        private bool _animateForward;

        private float _animationProgress;
        private Transform _objectTransform;

        private Rigidbody _rb;

        private void Awake()
        {
            _animateForward = playOnAwake;
            _rb = useRigidbody ? GetComponent<Rigidbody>() : null;

            _objectTransform = objectToAnimate ? objectToAnimate.transform : this.transform;
            animationData.ForEach(data =>
            {
                if (data.useDirection)
                {
                    data.positionFrom = _objectTransform.localPosition;
                    data.positionTo = data.positionFrom + data.direction;
                }
            });
        }

        protected override void TimeScaledUpdate(float timeScaledDeltaTime)
        {
            for (int i = 0; i < animationData.Count; i++)
            {
                AnimationData data = animationData[i];
                if (_animationProgress > i + 1)
                {
                    FinishAnimation(data);
                    continue;
                }

                ExecuteAnimation(data, _animationProgress - i);
                _animationProgress += timeScaledDeltaTime / data.animationTime * (_animateForward ? 1 : -1);
                break;
            }

            _animationProgress = Mathf.Clamp(_animationProgress, 0, animationData.Count);

            if (pingPongBack && _animationProgress >= animationData.Count)
                _animateForward = !_animateForward;

            if (pingPongBack && loop && _animationProgress <= 0)
                _animateForward = !_animateForward;

            if (loop && !pingPongBack && _animationProgress >= animationData.Count)
                _animationProgress = 0;
        }

        public void AnimateForward() => _animateForward = true;
        public void AnimateBackwards() => _animateForward = false;
        public void ToggleAnimation() => _animateForward = !_animateForward;


        private void ExecuteAnimation(AnimationData data, float animationProgress)
        {
            if (data.animatePosition)
            {
                if (useRigidbody)
                    _rb.MovePosition(Vector3.Lerp(data.positionFrom, data.positionTo,
                        data.animationCurve.Evaluate(animationProgress)));
                else
                    _objectTransform.localPosition =
                        Vector3.Lerp(data.positionFrom, data.positionTo,
                            data.animationCurve.Evaluate(animationProgress));
            }

            if (data.animateScale)
                _objectTransform.localScale = Vector3.Lerp(data.scaleFrom, data.scaleTo,
                    data.animationCurve.Evaluate(animationProgress));

            if (data.animateRotation)
            {
                if (useRigidbody)
                    _rb.MoveRotation(Quaternion.Euler(Vector3.Lerp(data.rotationFrom, data.rotationTo,
                        data.animationCurve.Evaluate(animationProgress))));
                else
                    _objectTransform.localRotation =
                        Quaternion.Euler(Vector3.Lerp(data.rotationFrom, data.rotationTo,
                            data.animationCurve.Evaluate(animationProgress)));
            }
        }

        private void FinishAnimation(AnimationData data)
        {
            if (data.animatePosition)
                _objectTransform.position = data.positionTo;
            if (data.animateScale)
                _objectTransform.localScale = data.scaleTo;
            if (data.animateRotation)
                _objectTransform.rotation = Quaternion.Euler(data.rotationTo);
        }


        [Serializable]
        private class AnimationData
        {
            [Header("Animation Settings")] public float animationTime;
            [Header("Position")] public bool animatePosition;
            public Vector3 positionFrom;
            public Vector3 positionTo;
            [Space(10)] public bool useDirection;
            public Vector3 direction;
            [Header("Scale")] public bool animateScale;
            public Vector3 scaleFrom;
            public Vector3 scaleTo;
            [Header("Rotation")] public bool animateRotation;
            public Vector3 rotationFrom;
            public Vector3 rotationTo;
            [Header("Animation Curve")] public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }
}