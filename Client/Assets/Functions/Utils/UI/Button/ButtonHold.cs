using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Events;

namespace Utils
{
    public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("Thời gian giữ để kích hoạt sự kiện hold (giây)")]
        public float holdTime = 1.25f;

        public UnityEvent onClick;
        public UnityEvent onHold;

        private CancellationTokenSource _cts;
        private bool isHolding = false;
        private bool wasHoldTriggered = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            isHolding = true;
            wasHoldTriggered = false;

            HoldCheckAsync(_cts.Token).Forget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _cts?.Cancel();

            if (wasHoldTriggered)
            {
                // Đã giữ đủ lâu và gọi onHold -> không gọi onClick
                return;
            }

            if (isHolding)
            {
                // Chỉ gọi onClick nếu vẫn đang giữ và chưa từng gọi onHold
                onClick?.Invoke();
            }
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
                if (isHolding)
                {
                    wasHoldTriggered = true;
                    onHold?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                // Bị hủy -> không làm gì
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
