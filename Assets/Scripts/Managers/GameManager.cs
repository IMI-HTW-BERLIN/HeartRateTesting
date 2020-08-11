using LocalServer;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Mi Band")] [SerializeField] private bool useMiBand;
        [SerializeField] private MiBand2Client miBand2Client;
        [SerializeField] private Volume postProcessingVolume;


        public bool UseMiBand => useMiBand;


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

        public void ChangeDepthOfFieldEffect(float value)
        {
            if (postProcessingVolume.profile.TryGet(out DepthOfField depthOfField))
                depthOfField.focusDistance.value = value;
        }
    }
}