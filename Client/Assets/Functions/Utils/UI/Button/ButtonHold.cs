using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Events;

namespace Utils
{
    [RequireComponent(typeof(Image))]
    public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("Thời gian giữ để kích hoạt sự kiện hold (giây)")]
        public float holdTime = 1.25f;

        public UnityEvent onClick;
        public UnityEvent onHold;

        private CancellationTokenSource _cts;
        private bool isHolding = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            isHolding = false;

            HoldCheckAsync(_cts.Token).Forget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isHolding)
            {
                // Đã gọi onHold rồi, không gọi onClick nữa
                _cts?.Cancel();
                return;
            }

            _cts?.Cancel();
            onClick?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cts?.Cancel();
            isHolding = false;
        }

        private async UniTaskVoid HoldCheckAsync(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(holdTime), cancellationToken: token);
                isHolding = true;
                onHold?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // Bị hủy thì thôi
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}