using System;
using DG.Tweening;
using Globals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool _isSpamClick = false;
        [SerializeField] private Transform _transform;

        private Vector3 _originalScale;

        private Tween _scaleUpTween;
        private Tween _scaleDownTween;

        private void Awake()
        {
            if (_transform == null)
                _transform = transform;

            _originalScale = _transform.localScale;

            // Tạo tween phồng lên
            _scaleUpTween = _transform.DOScale(_originalScale * 1.1f, 0.15f)
                .SetAutoKill(!_isSpamClick) // nếu spam thì không auto kill
                .SetEase(Ease.OutQuad)
                .Pause();

            // Tạo tween thu nhỏ về gốc
            _scaleDownTween = _transform.DOScale(_originalScale, 0.2f)
                .SetAutoKill(!_isSpamClick)
                .SetEase(Ease.InQuad)
                .Pause();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isSpamClick)
            {
                _scaleUpTween.Kill();
                _scaleUpTween = _transform.DOScale(_originalScale * 1.1f, 0.15f)
                    .SetAutoKill(true)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                _scaleDownTween.Pause();
            }

            _scaleUpTween.Restart();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isSpamClick)
            {
                _scaleDownTween.Kill();
                _scaleDownTween = _transform.DOScale(_originalScale, 0.2f)
                    .SetAutoKill(true)
                    .SetEase(Ease.InQuad);
            }
            else
            {
                _scaleUpTween.Pause();
            }

            _scaleDownTween.Restart();

            // Chỗ này bạn tùy chọn chơi sound khi thả ra
            Global.Instance?.Get<SoundManager>()?.PlaySoundOneShot("FX_Touch");
        }

        private void OnDisable()
        {
            _scaleUpTween.Kill();
            _scaleDownTween.Kill();

            if (_transform != null)
                _transform.localScale = _originalScale;
        }
    }
}