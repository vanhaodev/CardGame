using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Popup
{
    public class PopupToast : Popup
    {
        [SerializeField] private Transform _transformHideZone;
        [SerializeField] private Transform _transformToast;
        [SerializeField] private ContentSizeFitter2 _sizeFitterToast;
        [SerializeField] private TextMeshProUGUI _txContent;
        private float _hideToastMoveDuration;

        public void SetContent(string content, float hideToastMoveDuration = 1)
        {
            _hideToastMoveDuration = hideToastMoveDuration;
            _transformToast.position = new Vector3(0, 3, 0);
            _txContent.text = content;
            _sizeFitterToast.UpdateSize();
        }

        public override async UniTask Show(float fadeDuration = 1)
        {
            await base.Show(fadeDuration);
            await UniTask.WaitForSeconds(1, cancellationToken: destroyCancellationToken);
            _transformToast.DOMove(_transformHideZone.position, _hideToastMoveDuration).SetEase(Ease.OutBack)
                .OnComplete(() => Close(0)).WithCancellation(cancellationToken: destroyCancellationToken);
        }
    }
}