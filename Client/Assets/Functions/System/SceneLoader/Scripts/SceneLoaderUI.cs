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
            }

            try
            {
                await _canvasGroup.DOFade(isShow ? 1f : 0f, 0.5f).ToUniTask(cancellationToken: _ctsShow.Token);
            }
            catch
            {
            }
        }

        public async UniTask SetProgress(float progress, CancellationToken token = default)
        {
            _ctsProgress?.Cancel(); // Hủy animation cũ nếu có
            _ctsProgress = new CancellationTokenSource();

            _txProgress.text = progress.ToString("0") + "%";

            try
            {
                _imageLoadingFill.DOFillAmount(progress, 0.2f).ToUniTask(cancellationToken: _ctsProgress.Token);
            }
            catch 
            {
                
            }
        }

        private void OnDestroy()
        {
            _ctsShow?.Cancel();
        }
    }
}