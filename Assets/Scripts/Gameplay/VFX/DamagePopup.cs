using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Pool;

namespace Gameplay.VFX
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textMesh;
        [SerializeField] private float _animationDuration = 0.8f;
        [SerializeField] private float _moveUpDistance = 2f;
        
        [Header("Normal Damage")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private float _normalScale = 1f;

        [Header("Critical Damage")]
        [SerializeField] private Color _critColor = Color.orange;
        [SerializeField] private float _critScale = 1.5f;

        private IObjectPool<DamagePopup> _pool;
        private Transform _mainCameraTransform;

        private void Awake()
        {
            if (Camera.main != null) _mainCameraTransform = Camera.main.transform;
        }

        public void Init(IObjectPool<DamagePopup> pool)
        {
            _pool = pool;
        }

        public void Setup(int damageAmount, Vector3 startPosition, bool isCritical)
        {
            transform.position = startPosition + Vector3.up * 1.5f;
            _textMesh.text = damageAmount.ToString();
            
            _textMesh.color = isCritical ? _critColor : _normalColor;
            transform.localScale = Vector3.one * (isCritical ? _critScale : _normalScale);
            
            Animate();
        }

        private void Animate()
        {
            transform.DOKill();
            _textMesh.DOKill();
            
            float randomX = Random.Range(-0.5f, 0.5f);
            Vector3 targetPos = transform.position + new Vector3(randomX, _moveUpDistance, 0);
            transform.DOMove(targetPos, _animationDuration).SetEase(Ease.OutCirc);
            
            if (transform.localScale.x > _normalScale)
            {
                transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, 5, 1);
            }
            
            _textMesh.DOFade(0f, _animationDuration).SetEase(Ease.InExpo).OnComplete(() => 
            {
                _pool.Release(this);
            });
        }

        private void LateUpdate()
        {
            if (_mainCameraTransform != null)
            {
                transform.LookAt(transform.position + _mainCameraTransform.forward);
            }
        }
    }
}