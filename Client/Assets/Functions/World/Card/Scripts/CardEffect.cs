using System;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using Globals;

namespace World.Card
{
    public class CardEffect : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        private Vector3 _realScale; // Lưu trữ kích thước thực tế của card
        private CancellationTokenSource _touchCancellation; // Cancellation riêng cho hiệu ứng touch

        private void Awake()
        {
            _realScale = _transform.localScale; // Lấy kích thước ban đầu của card
        }

        public async void PlayTouchEffect()
        {
            // Nếu có hiệu ứng bubble đang chạy, huỷ bỏ nó
            _touchCancellation?.Cancel();

            // Tạo CancellationTokenSource mới cho hiệu ứng Bubble
            _touchCancellation = new CancellationTokenSource();

            try
            {
                GlobalFunction.Instance.Get<SoundManager>().PlaySoundOneShot(3);
                // Phóng to và thu nhỏ với hiệu ứng bubble
                await _transform.DOScale(_realScale * 1.2f, 0.2f) // Phóng to 1.2 lần kích thước thực tế trong 0.2s
                    // .SetEase(Ease.OutBack)  // Ease OutBack để tạo cảm giác bật lại
                    .AsyncWaitForCompletion();

                // Thu nhỏ lại về kích thước ban đầu
                await _transform.DOScale(_realScale, 0.2f) // Thu nhỏ lại về kích thước ban đầu
                    // .SetEase(Ease.InBack)  // Ease InBack để tạo cảm giác dừng lại
                    .AsyncWaitForCompletion();
            }
            catch (System.OperationCanceledException)
            {
                // Nếu hiệu ứng bị huỷ bỏ, không làm gì cả
                Debug.Log("Bubble effect cancelled");
            }
        }
    }
}