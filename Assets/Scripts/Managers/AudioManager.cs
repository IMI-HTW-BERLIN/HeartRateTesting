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
        
        private readonly Dictionary<AudioEnum, AudioData> _audioDict = new Dictionary<AudioEnum, AudioData>();

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            foreach (AudioData audioData in audioTypes.SelectMany(audioType => audioType.AudioData))
            {
                _audioDict.Add(audioData.GetAudioEnum(), audioData);
            }
        }

        public void PlayAudio(AudioEnum audioEnum, float delay)
        {
            _audioSource.clip = GetClip(audioEnum);
            _audioSource.PlayDelayed(delay);
        }

        public void PlayAudio(AudioEnum audioEnum) => _audioSource.PlayOneShot(GetClip(audioEnum));

        private AudioClip GetClip(AudioEnum audioEnum) => _audioDict[audioEnum].AudioClip;
    }
}