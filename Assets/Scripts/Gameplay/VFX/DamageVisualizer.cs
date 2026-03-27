using DG.Tweening;
using UnityEngine;

namespace Gameplay.VFX
{
    public class DamageVisualizer : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Renderer[] _renderers;

        [SerializeField] private Transform _modelTransform;

        [Header("Settings")] [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.1f;
        [SerializeField] private float _punchScale = 0.2f;

        [Header("Death Settings")] [SerializeField]
        private Color _deadColor = Color.gray;

        private MaterialPropertyBlock _mpb;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private Color[] _originalColors;
        private Tween _flashResetTween;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();

            _originalColors = new Color[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalColors[i] = _renderers[i].sharedMaterial.GetColor(ColorProperty);
            }
        }

        public void PlayHitEffect()
        {
            _flashResetTween.Kill();
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;
                _renderers[i].GetPropertyBlock(_mpb);
                _mpb.SetColor(ColorProperty, _flashColor);
                _renderers[i].SetPropertyBlock(_mpb);
            }

            _flashResetTween = DOVirtual.DelayedCall(_flashDuration, ResetColor);

            _modelTransform.DOKill(true);
            _modelTransform.DOPunchScale(Vector3.one * _punchScale, 0.2f, 10, 1);
        }

        public Color GetOriginalColor() => (_originalColors != null && _originalColors.Length > 0) ? _originalColors[0] : Color.white;

        public void SetDeadColor()
        {
            _flashResetTween.Kill();
            
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;
                _renderers[i].GetPropertyBlock(_mpb);
                _mpb.SetColor(ColorProperty, _deadColor);
                _renderers[i].SetPropertyBlock(_mpb);
            }
        }

        public void ResetColor()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;
                _renderers[i].GetPropertyBlock(_mpb);
                _mpb.SetColor(ColorProperty, _originalColors[i]);
                _renderers[i].SetPropertyBlock(_mpb);
            }
        }
    }
}