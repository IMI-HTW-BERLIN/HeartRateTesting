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
        [SerializeField] private bool useFakeHeartRate;
        [SerializeField] [Range(0, 140)] private int fakeHeartRate;

        /// <summary>
        /// The base heart rate is the players "normal" heart rate.
        /// To decrease difficulty, this will be the highest measured average.
        /// </summary>
        public int BaseHeartRate { get; private set; }

        public int CurrentHeartRate { get; private set; }
        public int CurrentHeartRateAverage { get; private set; }

        private readonly LimitedList<int> _heartRates = new LimitedList<int>(10);

        private float _lastTimePlayed;

        private void OnEnable()
        {
            if (!useFakeHeartRate)
                MiBand2Client.OnHeartRateChange += OnHeartRateChange;
            else
            {
                InvokeRepeating(nameof(OnFakeHeartRate), 0, 2f);
            }
        }

        private void OnFakeHeartRate() => OnHeartRateChange(new HeartRateResponse(fakeHeartRate, false, 0));

        private void OnDisable()
        {
            if (!useFakeHeartRate)
                MiBand2Client.OnHeartRateChange -= OnHeartRateChange;
        }

        private void Start() => StartCoroutine(StartPlayingHeartSound());

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
                AudioManager.Instance.PlayAudio(AudioEnum.FirstHeartSound);
                float rangeFactor = Math.Min(1, 1 - (float) CurrentHeartRate / 120);
                AudioManager.Instance.PlayAudio(AudioEnum.SecondHeartSound, range.GetInBetween(rangeFactor));
            }
        }
    }
}