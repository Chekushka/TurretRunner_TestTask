using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Gameplay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Appearance")]
        [SerializeField] private float _showDelay = 0.5f;
        [SerializeField] private float _showDuration = 0.4f;
        
        [Header("Hover Settings")]
        [SerializeField] private float _hoverScale = 1.1f;
        [SerializeField] private float _hoverDuration = 0.2f;

        [Header("Click Settings")]
        [SerializeField] private float _clickScale = 0.9f;
        [SerializeField] private Vector3 _punchRotation = new Vector3(0, 0, 5f);

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector3 _initialScale;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _initialScale = _rectTransform.localScale;
        }

        private void OnEnable()
        {
            _canvasGroup.alpha = 0f;
            _rectTransform.localScale = Vector3.zero;
            
            _canvasGroup.DOFade(1f, _showDuration).SetDelay(_showDelay).SetUpdate(true);
            _rectTransform.DOScale(_initialScale, _showDuration)
                .SetDelay(_showDelay)
                .SetEase(Ease.OutBack)
                .SetUpdate(true); 
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _rectTransform.DOKill();
            _rectTransform.DOScale(_initialScale * _hoverScale, _hoverDuration).SetUpdate(true);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _rectTransform.DOKill();
            _rectTransform.DOScale(_initialScale, _hoverDuration).SetUpdate(true);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _rectTransform.DOKill();
            _rectTransform.DOScale(_initialScale * _clickScale, 0.1f).SetUpdate(true);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _rectTransform.DOKill();
            _rectTransform.DOScale(_initialScale * _hoverScale, 0.1f).SetUpdate(true);
            
            _rectTransform.DOPunchRotation(_punchRotation, 0.3f, 10, 1).SetUpdate(true);
        }

        private void OnDisable()
        {
            _rectTransform.DOKill();
            _canvasGroup.DOKill();
        }
    }
}