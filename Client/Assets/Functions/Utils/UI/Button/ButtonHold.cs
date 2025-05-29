using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Utils
{
    public abstract class ButtonHold : MonoBehaviour
    {
        [Tooltip("Thời gian giữ để kích hoạt sự kiện hold (giây)")]
        public float holdTime = 1.25f;

        public UnityEvent onClick;
        public UnityEvent onHold;

        protected bool isHolding = false;
        protected bool wasHoldTriggered = false;
        protected CancellationTokenSource _cts;

        protected void StartHold()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            isHolding = true;
            wasHoldTriggered = false;

            HoldCheckAsync(_cts.Token).Forget();
        }

        protected void ReleaseHold()
        {
            _cts?.Cancel();

            if (!wasHoldTriggered && isHolding)
            {
                onClick?.Invoke();
            }

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
            catch (OperationCanceledException) { }
        }

        protected virtual void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}