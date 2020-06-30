using System;
using System.Collections;
using LocalServer;
using UnityEngine;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Mi Band")] [SerializeField] private bool useMiBand;
        [SerializeField] private MiBand2Client miBand2Client;

        [Header("Slow Motion")] [SerializeField]
        private float timeScaleFactor;

        [SerializeField] private float minDeltaForSlowMotion;
        [SerializeField] private float timeScaleDeltaForChange;


        private float _currentTimeScale = 1;
        private bool _isAnimating;

        private void OnEnable()
        {
            if(useMiBand)
                miBand2Client.StartMiBand();
        }

        private void OnDisable()
        {
            if(useMiBand)
                miBand2Client.StopMiBand();
        }

        private void Update()
        {
            if (HeartRateManager.Instance.BaseHeartRate == 0)
                return;

            float heartRate = HeartRateManager.Instance.CurrentHeartRateAverage;
            float baseHeartRate = HeartRateManager.Instance.BaseHeartRate;
            float heartRateDelta = Math.Min(1, heartRate / baseHeartRate);

            if (heartRateDelta > minDeltaForSlowMotion)
                return;

            float newTimeScale = Math.Min(1, heartRateDelta * timeScaleFactor);

            float timeScaleDelta = Math.Abs(newTimeScale - _currentTimeScale);
            if (timeScaleDelta < timeScaleDeltaForChange)
                return;

            if (_isAnimating)
                return;
            _isAnimating = true;
            StartCoroutine(FadeToTimeScale(newTimeScale, 2f));
        }

        private IEnumerator FadeToTimeScale(float newTimeScale, float animationTime)
        {
            float currentTimeScale = Time.timeScale;
            Debug.Log(newTimeScale);
            float time = 0;
            yield return new WaitUntil(() =>
            {
                time += Time.unscaledDeltaTime;
                float newValue = Mathf.Lerp(currentTimeScale, newTimeScale, time / animationTime);
                Time.timeScale = newValue;


                return time >= animationTime;
            });
            Debug.Log(Time.timeScale);
            _currentTimeScale = newTimeScale;
            _isAnimating = false;
        }
    }
}