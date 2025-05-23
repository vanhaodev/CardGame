using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    [System.Serializable]
    public class SmoothVitalFill
    {
        [SerializeField] private Image _spriteEchoFill;
        [SerializeField] private Image _spriteFill;
        private CancellationTokenSource _cts;

        public async void UpdateFill(int current, int max)
        {
            float target = Mathf.Clamp01((float)current / max);
            float currentFillAmount = _spriteFill.fillAmount;
            if (Mathf.Approximately(target, currentFillAmount)) return;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            if (target < currentFillAmount)
            {
                // Mất máu: fill giảm ngay, echo giảm mượt
                _spriteFill.fillAmount = target;
                await UniTask.WaitForSeconds(0.3f, cancellationToken: _cts.Token);
                _spriteEchoFill.DOFillAmount(target, 0.5f).SetEase(Ease.OutQuad)
                    .ToUniTask(cancellationToken: _cts.Token);
            }
            else if (target > currentFillAmount)
            {
                // Hồi máu: echo tăng ngay, fill tăng mượt
                _spriteEchoFill.fillAmount = target;
                await UniTask.WaitForSeconds(0.3f, cancellationToken: _cts.Token);
                _spriteFill.DOFillAmount(target, 0.5f).SetEase(Ease.OutQuad).ToUniTask(cancellationToken: _cts.Token);
            }
        }
    }
}