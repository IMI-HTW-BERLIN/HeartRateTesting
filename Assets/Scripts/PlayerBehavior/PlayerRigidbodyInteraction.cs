using UnityEngine;

namespace PlayerBehavior
{
    [RequireComponent(typeof(Collider))]
    public class PlayerRigidbodyInteraction : MonoBehaviour
    {
        [SerializeField] private Player player;

        private Rigidbody _currentRb;

        private void Update()
        {
            if (_currentRb != null)
                player.MovePlayer(_currentRb.velocity * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Rigidbody rb))
                _currentRb = rb;
        }

        private void OnTriggerExit(Collider other) =>
            _currentRb = null;
    }
}