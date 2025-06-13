using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Utils
{
    public partial class UIShaker
    {
        private List<RectTransform> _targets;
        private Vector3[] _originalScales;
        private Vector2[] _originalPositions;
        private readonly List<Tween> _activeTweens = new();

        public void StartWobble(List<RectTransform> targets, float range = 5f, float moveDuration = 1f,
            float extraScaleBuffer = 1.05f)
        {
            StopWobble();

            _targets = targets;
            _originalPositions = SaveOriginalPositions(targets);
            _originalScales = new Vector3[targets.Count];

            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];
                if (t == null) continue;

                _originalScales[i] = t.localScale;

                float width = t.rect.width;
                float height = t.rect.height;

                float originalHalfDiagonal = Mathf.Sqrt(width * width + height * height) / 2f;
                float maxOffset = Mathf.Sqrt(2f) * range;
                float scaleFactor = (originalHalfDiagonal + maxOffset) / originalHalfDiagonal;

                scaleFactor *= extraScaleBuffer;
                t.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
                t.DOKill();
            }

            // Lặp offset dùng chung cho tất cả
            void LoopSharedMove()
            {
                if (_targets == null) return;

                Vector2 offset = new Vector2(
                    UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range)
                );

                for (int i = 0; i < _targets.Count; i++)
                {
                    var t = _targets[i];
                    if (t == null) continue;

                    Vector2 originalPos = _originalPositions[i];

                    var tween = t.DOAnchorPos(originalPos + offset, moveDuration)
                        .SetEase(Ease.InOutSine);
                    if (i == 0) // Chỉ gắn OnComplete vào 1 tween để lặp
                    {
                        tween.OnComplete(LoopSharedMove);
                    }
                }
            }

            LoopSharedMove();
        }

        public void StopWobble()
        {
            if (_targets != null)
            {
                foreach (var t in _targets)
                    t?.DOKill();

                if (_originalPositions != null && _originalScales != null)
                {
                    for (int i = 0; i < _targets.Count; i++)
                    {
                        var t = _targets[i];
                        if (t == null) continue;

                        t.anchoredPosition = _originalPositions[i];
                        t.localScale = _originalScales[i];
                    }
                }
            }

            foreach (var tween in _activeTweens)
                tween?.Kill();

            _activeTweens.Clear();

            _targets = null;
            _originalPositions = null;
            _originalScales = null;
        }


        /// <summary>
        /// Lưu lại vị trí gốc của các target.
        /// </summary>
        private Vector2[] SaveOriginalPositions(List<RectTransform> targets)
        {
            Vector2[] result = new Vector2[targets.Count];
            for (int i = 0; i < targets.Count; i++)
                result[i] = targets[i]?.anchoredPosition ?? Vector2.zero;

            return result;
        }

        /// <summary>
        /// Trả các target về vị trí ban đầu.
        /// </summary>
        private void ResetPositions(List<RectTransform> targets, Vector2[] original)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] != null)
                    targets[i].anchoredPosition = original[i];
            }
        }
    }

    public partial class UIShaker
    {
    }
}