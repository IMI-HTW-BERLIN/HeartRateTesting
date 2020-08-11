using Audio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class SlowMotionManager : SceneSingleton<SlowMotionManager>
    {
        [Header("Slow Motion")] [SerializeField]
        private float minDeltaForSlowMotion;

        [SerializeField] private float timeScaleFactor;

        [Header("Post Processing")] [SerializeField]
        private Volume postProcessingVolume;

        [SerializeField] private float animationTime;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float chromaticAberrationIntensity;

        [Header("Time Scale Audio Effects")] [SerializeField]
        private float timeScalePitchInfluence;

        [SerializeField] private AnimationCurve timeScaleLowPassInfluence;
        [SerializeField] private float timeScaleSpatialVolumeInfluence;

        public float TimeScale { get; private set; }

        private ChromaticAberration _chromaticAberration;

        private float _animationProgress;
        private bool _timeIsSlowed;

        private const int LOWPASS_MAX = 22000;

        protected override void Awake()
        {
            base.Awake();
            if (postProcessingVolume.profile.TryGet(out ChromaticAberration chromaticAberration))
                _chromaticAberration = chromaticAberration;
        }

        private void Update()
        {
            if (HeartRateManager.Instance.CurrentHeartRateDelta < minDeltaForSlowMotion)
            {
                _animationProgress += Time.deltaTime / animationTime;
                TimeScale = HeartRateManager.Instance.CurrentHeartRateDelta * timeScaleFactor;
                if (!_timeIsSlowed)
                {
                    _timeIsSlowed = true;
                    AudioManager.Instance.PlayAudio(AudioEnum.SlowMotionSounds.SlowDownTime);
                }
            }
            else
            {
                _animationProgress -= Time.deltaTime;
                TimeScale = 1;
                if (_timeIsSlowed)
                {
                    _timeIsSlowed = false;
                    AudioManager.Instance.PlayAudio(AudioEnum.SlowMotionSounds.SpeedUpTime);
                }
            }

            _animationProgress = Mathf.Clamp01(_animationProgress);

            _chromaticAberration.intensity.value =
                animationCurve.Evaluate(_animationProgress) * chromaticAberrationIntensity;

            // Change Pitch
            AudioManager.Instance.ChangeMixerEffect(AudioManager.MixerGroup.PitchChangeable,
                AudioManager.MixerEffect.PitchChangeablePitch, 1 - (1 - TimeScale) * timeScalePitchInfluence);

            // Change Lowpass (deepen voices)
            float newLowPassValue = timeScaleLowPassInfluence.Evaluate(TimeScale) * LOWPASS_MAX;
            AudioManager.Instance.ChangeMixerEffect(AudioManager.MixerGroup.Spatial,
                AudioManager.MixerEffect.SpatialLowpass, newLowPassValue);

            // Change volume (easier to hear door-code)
            AudioManager.Instance.ChangeMixerEffect(AudioManager.MixerGroup.Spatial,
                AudioManager.MixerEffect.SpatialVolume, -5 - (1 - TimeScale) * timeScaleSpatialVolumeInfluence);
        }
    }
}