using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class AudioType
    {
        [SerializeField] private string audioDataName;
        [SerializeField] private List<AudioData> audioData = new List<AudioData>();
        
        public static string AudioDataName => nameof(audioDataName);
        public static string AudioDataListName => nameof(audioData);

        public List<AudioData> AudioData => audioData;
    }
}