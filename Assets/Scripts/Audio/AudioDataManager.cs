using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    public class AudioDataManager : MonoBehaviour
    {
        [SerializeField] private List<AudioType> audioTypes = new List<AudioType>();
        public static string AudioTypeListName => nameof(audioTypes);

        public List<AudioType> AudioTypes => audioTypes;

        private readonly Dictionary<string, AudioData> _audioDict = new Dictionary<string, AudioData>();

        private void Awake()
        {
            foreach (AudioData audioData in audioTypes.SelectMany(audioType => audioType.AudioData))
            {
                _audioDict.Add(audioData.AudioName, audioData);
            }
        }

        public AudioClip GetClip(Enum audioEnum)
        {
            if (!_audioDict.ContainsKey(audioEnum.ToString()))
                throw new ArgumentException("Could not find audio enum. Make sure to only use enums from AudioEnum.cs");
            return _audioDict[audioEnum.ToString()].AudioClip;
        }
    }
}