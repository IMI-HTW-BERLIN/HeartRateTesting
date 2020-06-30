using Data.ResponseTypes;
using LocalServer;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HeartRateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtHeartRate;
        [SerializeField] private TextMeshProUGUI txtBaseHeartRate;

        private void Update()
        {
            txtBaseHeartRate.SetText(HeartRateManager.Instance.BaseHeartRate.ToString());
        }

        private void OnEnable()
        {
            MiBand2Client.OnHeartRateChange += OnHeartRateChange;
        }

        private void OnDisable()
        {
            MiBand2Client.OnHeartRateChange -= OnHeartRateChange;
        }

        private void OnHeartRateChange(HeartRateResponse heartRateResponse)
        {
            txtHeartRate.SetText(heartRateResponse.HeartRate.ToString());
        }
    }
}