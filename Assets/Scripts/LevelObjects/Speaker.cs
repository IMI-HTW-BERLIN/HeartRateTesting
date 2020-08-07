using System;
using Audio;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelObjects
{
    public class Speaker : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private void Start()
        {
            PlayRandomAudio();
        }

        private void PlayRandomAudio()
        {
            int numberOfAudios = Enum.GetNames(typeof(AudioEnum.RandomAudio)).Length;
            int randomNumber = Random.Range(0, numberOfAudios);
            audioSource.clip = AudioManager.Instance.GetAudioClip((AudioEnum.RandomAudio) randomNumber);
            audioSource.Play();
            CoroutineManager.Instance.WaitUntil(() => !audioSource.isPlaying, PlayRandomAudio);
        }
    }
}