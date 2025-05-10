using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Globals;
using UnityEngine;
using UnityEngine.Events;

namespace Popup
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Popup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        public UnityAction OnShow;
        public UnityAction OnHide;

        protected virtual void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual async UniTask SetupData()
        {
            _canvasGroup.alpha = 0;
            gameObject.transform.SetAsLastSibling();
        }

        public virtual async UniTask Show(float fadeDuration = 1)
        {
            await _canvasGroup.DOFade(1, fadeDuration).WithCancellation(cancellationToken: destroyCancellationToken);
            OnShow?.Invoke();
        }

        public virtual void Close(float fadeDuration = 1)
        {
            _canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    Global.Instance.Get<PopupManager>().ClosePopup(this);
                    OnHide?.Invoke();
                    OnShow = null;
                    OnHide = null;
                })
                .WithCancellation(cancellationToken: destroyCancellationToken);
        }
    }
}