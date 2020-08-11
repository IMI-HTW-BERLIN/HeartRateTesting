using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class HighlightManager : SceneSingleton<HighlightManager>
    {
        [Header("Heart Rate Settings")] [SerializeField]
        private float minHeartRateDelta;

        [SerializeField] private AnimationCurve hueChangeAnimationCurve;
        [SerializeField] private float hueChangeAnimationTime;
        [SerializeField] private Volume postProcessingHighlighting;
        [SerializeField] private AnimationCurve hueHighlightCurve;

        private ColorCurves _colorCurves;
        private float _animationProcess;

        protected override void Awake()
        {
            base.Awake();
            if (postProcessingHighlighting.profile.TryGet(out ColorCurves colorCurves))
                _colorCurves = colorCurves;

            _colorCurves.hueVsSat.overrideState = true;
            for (int i = 0; i < hueHighlightCurve.length; i++)
            {
                _colorCurves.hueVsSat.value.AddKey(hueHighlightCurve[i].time, 0.5f);
                Keyframe keyframe = _colorCurves.hueVsSat.value[i];
                keyframe.inTangent = 0;
                keyframe.outTangent = 0;
                _colorCurves.hueVsSat.value.MoveKey(i, keyframe);
            }

            _colorCurves.satVsSat.overrideState = true;
            _colorCurves.satVsSat.value.AddKey(1, 0.5f);
        }

        private void Update()
        {
            if (HeartRateManager.Instance.CurrentHeartRateDelta <= minHeartRateDelta)
                _animationProcess += Time.deltaTime / hueChangeAnimationTime;
            else
                _animationProcess -= Time.deltaTime / hueChangeAnimationTime;

            _animationProcess = Mathf.Clamp01(_animationProcess);

            for (int i = 0; i < _colorCurves.hueVsSat.value.length; i++)
            {
                _colorCurves.hueVsSat.value.MoveKey(i,
                    new Keyframe(_colorCurves.hueVsSat.value[i].time,
                        Mathf.Lerp(0.5f, hueHighlightCurve[i].value,
                            hueChangeAnimationCurve.Evaluate(_animationProcess))));
            }

            _colorCurves.satVsSat.value.MoveKey(0, new Keyframe(1, Mathf.Lerp(0.5f, 1,
                hueChangeAnimationCurve.Evaluate(_animationProcess))));
        }
    }
}