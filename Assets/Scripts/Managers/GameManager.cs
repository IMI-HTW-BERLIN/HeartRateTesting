using LocalServer;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Mi Band")] [SerializeField] private bool useMiBand;
        [SerializeField] private MiBand2Client miBand2Client;
        [SerializeField] private Volume postProcessingVolume;

        [Header("Scene Loading")] [SerializeField]
        private Animator animator;

        [SerializeField] private float sceneAnimationTime;

        [SerializeField] private TextMeshProUGUI txtSceneName;

        private static readonly int FadeIn = Animator.StringToHash("FadeIn");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");


        public bool UseMiBand => useMiBand;


        private void OnEnable()
        {
            if (useMiBand)
                miBand2Client.StartMiBand();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            if (useMiBand)
                miBand2Client.StopMiBand();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void ChangeDepthOfFieldEffect(float value)
        {
            if (postProcessingVolume.profile.TryGet(out DepthOfField depthOfField))
                depthOfField.focusDistance.value = value;
        }

        public void LoadScene(int sceneIndex)
        {
            animator.SetTrigger(FadeIn);
            CoroutineManager.Instance.WaitForSeconds(sceneAnimationTime, () => SceneManager.LoadScene(sceneIndex));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            txtSceneName.SetText(scene.name);
            animator.SetTrigger(FadeOut);
        }
    }
}