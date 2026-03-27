using UnityEngine;
using DG.Tweening;

namespace Gameplay.UI
{
    public class TutorialHandUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private RectTransform _handRectTransform;
        [SerializeField] private float _swipeDistance = 300f;
        [SerializeField] private float _duration = 1.2f;

        private void OnEnable()
        {
            _handRectTransform.DOAnchorPosX(_swipeDistance, _duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            _handRectTransform.DOKill();
        }
    }
}