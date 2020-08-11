using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider))]
    public class Trampoline : MonoBehaviour
    {
        [SerializeField] private float jumpBoost;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerBehavior.Player player))
                player.AddVelocity(jumpBoost * Vector3.up, true);
        }
    }
}