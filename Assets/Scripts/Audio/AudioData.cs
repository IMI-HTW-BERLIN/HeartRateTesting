using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class AudioData
    {
        [SerializeField] private string audioName;
        [SerializeField] private AudioClip audioClip;
        public static string AudioNameFieldName => nameof(audioName);
        public static string AudioClipFieldName => nameof(audioClip);

        public string AudioName => audioName;
        public AudioClip AudioClip => audioClip;
    }
}