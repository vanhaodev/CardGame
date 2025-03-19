using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace System.SceneLoader
{
    public class SceneLoaderUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _txProgress;
        [SerializeField] private Image _imageLoadingFill;
        [SerializeField] CanvasGroup _canvasGroup;
        private CancellationTokenSource _ctsShow;
        private CancellationTokenSource _ctsProgress;

        public async UniTask Show(bool isShow)
        {
            _ctsShow?.Cancel(); // Hủy animation cũ nếu có
            _ctsShow = new CancellationTokenSource();
            if (isShow)
            {
                _imageLoadingFill.fillAmount = 0;
                _txProgress.text = "0%";
                gameObject.SetActive(true);
            }

            try
            {
                await _canvasGroup.DOFade(isShow ? 1f : 0f, 0.5f).ToUniTask(cancellationToken: _ctsShow.Token);
            }
            catch
            {
            }

            if (!isShow)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetProgress(float targetProgress)
        {
            _ctsProgress?.Cancel();
            _ctsProgress = new CancellationTokenSource();
            var token = _ctsProgress.Token;

            float currentProgress = _imageLoadingFill.fillAmount;
            float duration = Mathf.Lerp(1f, 2f, currentProgress); // Gần 100% thì chậm lại

            var tween = DOVirtual.Float(currentProgress, targetProgress, duration, value =>
                {
                    if (token.IsCancellationRequested) return;
                    _imageLoadingFill.fillAmount = value;
                    _txProgress.text = $"{Mathf.RoundToInt(value * 100)}%";
                })
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);

            token.Register(() => tween.Kill()); // Hủy tween khi token bị hủy
            Debug.Log(_imageLoadingFill.fillAmount);
        }

        public async UniTask WaitFillAmountFull()
        {
            await UniTask.WaitUntil(() => (int)_imageLoadingFill.fillAmount >= 1);
            await UniTask.WaitForSeconds(1);
        }
        private void OnDestroy()
        {
            _ctsShow?.Cancel();
        }
    }
}