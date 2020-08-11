using System;
using System.Collections;
using Audio;
using Data.ResponseTypes;
using LocalServer;
using UnityEngine;
using Utils;

namespace Managers
{
    public class HeartRateManager : Singleton<HeartRateManager>
    {
        [Range(0, 160)] [SerializeField] private int heartRate;

        /// <summary>
        /// The base heart rate is the players "normal" heart rate.
        /// To decrease difficulty, this will be the highest measured average.
        /// </summary>
        public int BaseHeartRate { get; private set; }

        public float CurrentHeartRateDelta { get; private set; } = 1;
        private int CurrentHeartRate { get; set; }
        private int CurrentHeartRateAverage { get; set; }


        private readonly LimitedList<int> _heartRates = new LimitedList<int>(10);

        private float _lastTimePlayed;

        private void OnEnable()
        {
            MiBand2Client.OnHeartRateChange += OnHeartRateChange;
        }

        private void OnDisable()
        {
            MiBand2Client.OnHeartRateChange -= OnHeartRateChange;
        }

        private void Start() => StartCoroutine(StartPlayingHeartSound());

        private void Update()
        {
            CurrentHeartRateDelta = 1;
            if (GameManager.Instance.UseMiBand && Instance.BaseHeartRate != 0)
            {
                float heartRate = Instance.CurrentHeartRateAverage;
                float baseHeartRate = Instance.BaseHeartRate;
                CurrentHeartRateDelta = Math.Min(1, heartRate / baseHeartRate);
            }
            else if (!GameManager.Instance.UseMiBand)
                CurrentHeartRateDelta = heartRate / 160f;
        }


        private void CalculateBaseHeartRate(int heartRate)
        {
            _heartRates.Add(heartRate);
            if (!_heartRates.IsFull)
                return;


            int avg = _heartRates.Average();
            CurrentHeartRateAverage = avg;
            BaseHeartRate = Math.Max(avg, BaseHeartRate);
        }

        private void OnHeartRateChange(HeartRateResponse heartRateResponse)
        {
            CalculateBaseHeartRate(heartRateResponse.HeartRate);
            CurrentHeartRate = heartRateResponse.HeartRate;
        }

        private IEnumerator StartPlayingHeartSound()
        {
            yield return new WaitUntil(() => BaseHeartRate != 0);
            Range range = new Range(0.2f, 0.5f);
            while (true)
            {
                yield return new WaitUntil(() => _lastTimePlayed + 1 / (CurrentHeartRate / 60f) < Time.unscaledTime);
                _lastTimePlayed = Time.unscaledTime;
                AudioManager.Instance.PlayAudio(AudioEnum.HeartSound.FirstHeartSound,
                    AudioManager.AudioSourceType.HeartRate);
                float rangeFactor = Math.Min(1, 1 - (float) CurrentHeartRate / 120);
                AudioManager.Instance.PlayAudio(AudioEnum.HeartSound.SecondHeartSound,
                    range.GetInBetween(rangeFactor), AudioManager.AudioSourceType.HeartRate);
            }
        }
    }
}