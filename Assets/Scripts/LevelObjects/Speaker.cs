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
        [SerializeField] private bool playRandomAudio;

        private void Start()
        {
            if (playRandomAudio)
                PlayRandomAudio();
        }

        public void PlayAudio(AudioClip audioClip) => audioSource.PlayOneShot(audioClip);
        public bool CanPlayAudio() => !audioSource.isPlaying;

        private void PlayRandomAudio()
        {
            int numberOfAudios = Enum.GetNames(typeof(AudioEnum.RandomAudio)).Length;
            int randomNumber = Random.Range(0, numberOfAudios);
            audioSource.clip = AudioManager.Instance.GetAudioClip((AudioEnum.RandomAudio) randomNumber);
            audioSource.Play();
            StartCoroutine(CoroutineManager.WaitUntilCoroutine(() => !audioSource.isPlaying, PlayRandomAudio));
        }
    }
}