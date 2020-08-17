using UnityEngine;

namespace Managers
{
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private int sceneIndexToSwitchTo;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerBehavior.Player _))
                GameManager.Instance.LoadScene(sceneIndexToSwitchTo);
        }
    }
}