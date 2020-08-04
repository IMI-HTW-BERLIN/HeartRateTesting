using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;
using AudioType = Audio.AudioType;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        public static string AudioTypeListName => nameof(audioTypes);

        [SerializeField] private List<AudioType> audioTypes = new List<AudioType>();

        public List<AudioType> AudioTypes => audioTypes;
        private AudioSource _audioSource;

        private readonly Dictionary<string, AudioData> _audioDict = new Dictionary<string, AudioData>();

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();

            foreach (AudioData audioData in audioTypes.SelectMany(audioType => audioType.AudioData))
            {
                _audioDict.Add(audioData.AudioName, audioData);
            }
        }

        public void PlayAudio(Enum audioEnum, float delay)
        {
            _audioSource.clip = GetClip(audioEnum);
            _audioSource.PlayDelayed(delay);
        }

        public void PlayAudio(Enum audioEnum) => _audioSource.PlayOneShot(GetClip(audioEnum));

        private AudioClip GetClip(Enum audioEnum) => _audioDict[audioEnum.ToString()].AudioClip;
    }
}