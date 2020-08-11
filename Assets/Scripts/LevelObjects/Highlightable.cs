using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider))]
    public class Highlightable : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color highlightColor;

        private Color _defaultColor;

        private void Awake() => _defaultColor = meshRenderer.material.color;

        public void ShowHighlight() => meshRenderer.material.color = highlightColor;

        public void HideHighlight() => meshRenderer.material.color = _defaultColor;
    }
}