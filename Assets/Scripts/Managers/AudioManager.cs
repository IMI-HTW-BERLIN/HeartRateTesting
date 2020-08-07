using System;
using Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioDataManager))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource audioSourceHeartRate;
        [SerializeField] private AudioMixerGroup masterAudioMixerGroup;
        [SerializeField] private AudioMixerGroup spatialAudioMixerGroup;

        [Header("Time Scale Effects")] [SerializeField]
        private float timeScalePitchInfluence;

        [SerializeField] private AnimationCurve timeScaleLowPassInfluence;
        [SerializeField] private float timeScaleSpatialVolumeInfluence;

        public enum AudioSourceType { HeartRate, General }


        private AudioSource _audioSourceGlobal;
        private AudioDataManager _audioDataManager;

        private const string MASTER_PITCH = "Master_Pitch";
        private const string SPATIAL_VOLUME = "Spatial_Volume";
        private const string SPATIAL_LOWPASS = "Spatial_Lowpass";
        private const int LOWPASS_MAX = 22000;
        private const int LOWPASS_MIN = 10;

        protected override void Awake()
        {
            base.Awake();
            _audioSourceGlobal = GetComponent<AudioSource>();
            _audioDataManager = GetComponent<AudioDataManager>();
        }

        private void Update()
        {
            masterAudioMixerGroup.audioMixer.SetFloat(MASTER_PITCH,
                1 - (1 - GameManager.Instance.TimeScale) * timeScalePitchInfluence);
            float newLowPassValue = timeScaleLowPassInfluence.Evaluate(GameManager.Instance.TimeScale) * LOWPASS_MAX;
            spatialAudioMixerGroup.audioMixer.SetFloat(SPATIAL_LOWPASS, newLowPassValue);
            spatialAudioMixerGroup.audioMixer.SetFloat(SPATIAL_VOLUME,
                0 - (1 - GameManager.Instance.TimeScale) * timeScaleSpatialVolumeInfluence);
        }

        public void PlayAudio(Enum audioEnum, float delay,
            AudioSourceType audioSourceType = AudioSourceType.General)
        {
            AudioClip clip = _audioDataManager.GetClip(audioEnum);
            switch (audioSourceType)
            {
                case AudioSourceType.General:
                    _audioSourceGlobal.clip = clip;
                    _audioSourceGlobal.PlayDelayed(delay);
                    break;
                case AudioSourceType.HeartRate:
                    audioSourceHeartRate.clip = clip;
                    audioSourceHeartRate.PlayDelayed(delay);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioSourceType), audioSourceType, null);
            }
        }

        public void PlayAudio(Enum audioEnum, AudioSourceType audioSourceType = AudioSourceType.General)
        {
            _audioSourceGlobal.PlayOneShot(_audioDataManager.GetClip(audioEnum));
            AudioClip clip = _audioDataManager.GetClip(audioEnum);
            switch (audioSourceType)
            {
                case AudioSourceType.General:
                    _audioSourceGlobal.PlayOneShot(clip);
                    break;
                case AudioSourceType.HeartRate:
                    audioSourceHeartRate.PlayOneShot(clip);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioSourceType), audioSourceType, null);
            }
        }

        public AudioClip GetAudioClip(Enum audioEnum) => _audioDataManager.GetClip(audioEnum);

        public bool CanPlayAudio(AudioSourceType audioSourceType = AudioSourceType.General)
        {
            switch (audioSourceType)
            {
                case AudioSourceType.General:
                    return !_audioSourceGlobal.isPlaying;
                case AudioSourceType.HeartRate:
                    return !audioSourceHeartRate.isPlaying;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioSourceType), audioSourceType, null);
            }
        }
    }
}