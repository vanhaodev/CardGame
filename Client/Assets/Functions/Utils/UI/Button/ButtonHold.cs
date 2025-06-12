using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Utils
{
    public abstract class ButtonHold : MonoBehaviour
    {
        [Tooltip("Thời gian giữ để kích hoạt hold (giây)")]
        public float holdTime = 1.25f;

        public UnityEvent onClick;
        public UnityEvent onHold;

        private CancellationTokenSource _cts;
        private bool _holdTriggered;

        protected void BeginHold()
        {
            CancelHold();
            _holdTriggered = false;
            _cts = new CancellationTokenSource();
            WaitHoldAsync(_cts.Token).Forget();
        }

        protected void EndHold(bool isClickCandidate)
        {
            if (!_holdTriggered && isClickCandidate)
            {
                onClick?.Invoke();
            }

            CancelHold();
        }

        protected void CancelHold()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async UniTaskVoid WaitHoldAsync(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(holdTime), cancellationToken: token);
                _holdTriggered = true;
                onHold?.Invoke();
            }
            catch (OperationCanceledException) { }
        }

        protected virtual void OnDestroy() => CancelHold();
    }
}