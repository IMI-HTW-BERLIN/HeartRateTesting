using System;
using LocalServer;
using UnityEngine;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Mi Band")] [SerializeField] private bool useMiBand;
        [SerializeField] private MiBand2Client miBand2Client;

        [Header("Slow Motion")] [SerializeField]
        private float slowMotionTimeScale;

        [SerializeField] private float minDeltaForSlowMotion;

        public float TimeScale { get; private set; } = 1f;

        private void OnEnable()
        {
            if (useMiBand)
                miBand2Client.StartMiBand();
        }

        private void OnDisable()
        {
            if (useMiBand)
                miBand2Client.StopMiBand();
        }

        private void Update()
        {
            if (HeartRateManager.Instance.BaseHeartRate == 0)
                return;

            float heartRate = HeartRateManager.Instance.CurrentHeartRateAverage;
            float baseHeartRate = HeartRateManager.Instance.BaseHeartRate;
            float heartRateDelta = Math.Min(1, heartRate / baseHeartRate);

            float newTimeScale = heartRateDelta < minDeltaForSlowMotion ? slowMotionTimeScale : 1f;

            TimeScale = newTimeScale;
        }
    }
}