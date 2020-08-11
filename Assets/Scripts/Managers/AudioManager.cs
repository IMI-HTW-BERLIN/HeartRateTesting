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
        [SerializeField] private AudioMixerGroup pitchChangeableAudioMixerGroup;
        [SerializeField] private AudioMixerGroup spatialAudioMixerGroup;

        public enum AudioSourceType { HeartRate, General }

        public enum MixerEffect { PitchChangeablePitch, SpatialVolume, SpatialLowpass }

        public enum MixerGroup { Master, PitchChangeable, Spatial }


        private AudioSource _audioSourceGlobal;
        private AudioDataManager _audioDataManager;


        protected override void Awake()
        {
            base.Awake();
            _audioSourceGlobal = GetComponent<AudioSource>();
            _audioDataManager = GetComponent<AudioDataManager>();
        }

        private void Update()
        {
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

        public void ChangeMixerEffect(MixerGroup group, MixerEffect effect, float value)
        {
            switch (group)
            {
                case MixerGroup.Master:
                    masterAudioMixerGroup.audioMixer.SetFloat(effect.ToString(), value);
                    break;
                case MixerGroup.PitchChangeable:
                    pitchChangeableAudioMixerGroup.audioMixer.SetFloat(effect.ToString(), value);
                    break;
                case MixerGroup.Spatial:
                    spatialAudioMixerGroup.audioMixer.SetFloat(effect.ToString(), value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(group), group, null);
            }
        }

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