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

        public static string AudioDataFieldName => nameof(audioDataName);
        public static string AudioDataListFieldName => nameof(audioData);

        public string AudioDataName => audioDataName;

        public List<AudioData> AudioData => audioData;
    }
}