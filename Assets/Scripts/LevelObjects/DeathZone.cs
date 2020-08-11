using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider))]
    public class DeathZone : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerBehavior.Player player))
                player.RespawnAt(respawnPoint.position);
        }
    }
}