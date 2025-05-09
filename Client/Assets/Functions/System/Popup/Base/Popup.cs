using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Globals;
using UnityEngine;

namespace Popup
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Popup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        protected virtual void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual async UniTask SetupData()
        {
            _canvasGroup.alpha = 0;
        }

        public virtual async UniTask Show()
        {
            await _canvasGroup.DOFade(1, 1).WithCancellation(cancellationToken: destroyCancellationToken);
        }

        public virtual void Close()
        {
            _canvasGroup.DOFade(0, 1).OnComplete(() => { Global.Instance.Get<PopupManager>().ClosePopup(this); })
                .WithCancellation(cancellationToken: destroyCancellationToken);
        }
    }
}