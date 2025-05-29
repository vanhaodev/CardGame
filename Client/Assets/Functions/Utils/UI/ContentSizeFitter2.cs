using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

namespace Utils
{
    /// <summary>
    /// Thay thế cho bản của unity không có khả năng neo theo Anchor Preset <br/>
    /// Cần gọi UpdateSize() để update lại size
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitter2 : MonoBehaviour
    {
        [SerializeField] bool _isUpdateWidth;
        [SerializeField] bool _isUpdateHeight;

        private RectTransform _rectTransform;

        /// <summary>
        /// if have child, pls update child content size fitter first
        /// </summary>
        [SerializeField] ContentSizeFitter2[] _childContentSizeFitters;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        [Button]
        public async UniTask UpdateSize()
        {
            if (!_isUpdateWidth && !_isUpdateHeight)
                throw new Exception("this sizer not select size type yet");

            if (_childContentSizeFitters != null)
            {
                foreach (var child in _childContentSizeFitters)
                {
                    await child.UpdateSize();
                }
            }

            await UniTask.NextFrame();

            var (newSize, newPos) = CalculateAdjustedSizeAndPosition();
            _rectTransform.sizeDelta = newSize;
            _rectTransform.anchoredPosition = newPos;
        }

        [Button]
        public async UniTask UpdateSizeAnimation(float duration = 0.25f, Ease ease = Ease.Linear)
        {
            if (!_isUpdateWidth && !_isUpdateHeight)
                throw new Exception("this sizer not select size type yet");

            if (_childContentSizeFitters != null)
            {
                foreach (var child in _childContentSizeFitters)
                {
                    await child.UpdateSizeAnimation();
                }
            }

            await UniTask.NextFrame();

            var (newSize, newPos) = CalculateAdjustedSizeAndPosition();
            // Tạo tween cho thay đổi vị trí
            var positionTween = DOTween.To(() => _rectTransform.anchoredPosition,
                    x => _rectTransform.anchoredPosition = x, newPos, duration).SetEase(ease)
                .WithCancellation(cancellationToken: this.destroyCancellationToken);

            // Tạo tween cho thay đổi kích thước
            var sizeTween = DOTween
                .To(() => _rectTransform.sizeDelta, x => _rectTransform.sizeDelta = x, newSize, duration).SetEase(ease)
                .WithCancellation(cancellationToken: this.destroyCancellationToken);

            // Chờ cả hai tween hoàn thành
            await UniTask.WhenAll(positionTween, sizeTween);
        }

        private (Vector2, Vector2) CalculateAdjustedSizeAndPosition()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            Vector2 anchorMin = _rectTransform.anchorMin;
            Vector2 anchorMax = _rectTransform.anchorMax;

            bool stretchX = anchorMin.x != anchorMax.x;
            bool stretchY = anchorMin.y != anchorMax.y;

            Vector2 originalSize = _rectTransform.sizeDelta;
            Vector2 originalPos = _rectTransform.anchoredPosition;
            Vector2 newSize = originalSize;

            if (_isUpdateWidth && !stretchX)
                newSize.x = LayoutUtility.GetPreferredSize(_rectTransform, 0);

            if (_isUpdateHeight && !stretchY)
                newSize.y = LayoutUtility.GetPreferredSize(_rectTransform, 1);

            Vector2 delta = newSize - originalSize;
            Vector2 anchorCenter = (anchorMin + anchorMax) / 2;

            Vector2 newPos = originalPos + new Vector2(
                delta.x * (_rectTransform.pivot.x - anchorCenter.x),
                delta.y * (_rectTransform.pivot.y - anchorCenter.y)
            );

            return (newSize, newPos);
        }
    }
}