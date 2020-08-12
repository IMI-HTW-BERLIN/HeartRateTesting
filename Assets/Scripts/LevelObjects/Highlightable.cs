using Managers;
using Settings;
using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider))]
    public class Highlightable : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color highlightColor = Consts.Colors.HIGHLIGHT_GREEN;
        [SerializeField] private float animationTime;
        [SerializeField] private AnimationCurve highlightAnimationCurve;

        private Color _defaultColor;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        private float _animationProgress;
        private bool _showHighlight;

        private void Awake() => _defaultColor = meshRenderer.material.GetColor(ColorProperty);

        private void Update()
        {
            // if (HeartRateManager.Instance.CurrentHeartRateDelta <= 0.8f)
            //     ShowHighlight();
            // else
            //     HideHighlight();
            //
            // if (_showHighlight)
            //     _animationProgress += Time.deltaTime / animationTime;
            // else
            //     _animationProgress -= Time.deltaTime / animationTime;
            //
            // _animationProgress = Mathf.Clamp01(_animationProgress);

            _animationProgress = 1 - HeartRateManager.Instance.CurrentHeartRateDelta;

            meshRenderer.material.SetColor(ColorProperty,
                Color.Lerp(_defaultColor, highlightColor, highlightAnimationCurve.Evaluate(_animationProgress)));
        }

        public void SetColor(Color color) => highlightColor = color;

        public void ShowHighlight() => _showHighlight = true;

        public void HideHighlight() => _showHighlight = false;
    }
}