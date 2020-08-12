using LevelObjects.UserInteraction;
using Managers;
using Settings;
using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider))]
    public class Mine : Activatable
    {
        [SerializeField] private bool isSafe;
        [SerializeField] private float deathDelay;
        [SerializeField] private Highlightable highlightable;

        private PlayerBehavior.Player _player;

        private void Awake()
        {
            highlightable.SetColor(isSafe ? Consts.Colors.HIGHLIGHT_GREEN : Consts.Colors.HIGHLIGHT_RED);
        }

        protected override void OnActivation()
        {
            if (isSafe)
                return;
            CoroutineManager.Instance.WaitForSeconds(deathDelay, () =>
            {
                if (_player)
                    _player.RespawnAt(HighlightManager.Instance.RespawnPoint.position);
            });
        }

        protected override void OnDeactivation()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerBehavior.Player player))
                _player = player;
        }
    }
}