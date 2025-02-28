using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public class CameraFitBoxCollider2D : MonoBehaviour
    {
        public Camera targetCamera; // Camera cần điều chỉnh
        public BoxCollider2D targetCollider; // Collider cần bao trọn
        public float padding = -1f; // Thêm một ít khoảng trống để không bị cắt

        private void Start()
        {
            Init();
        }

        async void Init()
        {
            await Task.Yield();
            if (targetCamera == null || targetCollider == null) return;

            // Lấy tọa độ các điểm biên của collider
            Bounds bounds = targetCollider.bounds;

            // Chiều rộng và chiều cao của collider trong thế giới
            float colliderWidth = bounds.size.x;
            float colliderHeight = bounds.size.y;

            // Tính aspect ratio của camera
            float screenRatio = (float)Screen.width / Screen.height;
            float targetSizeByHeight = colliderHeight / 2f + padding;
            float targetSizeByWidth = (colliderWidth / 2f + padding) / screenRatio;

            // Chọn kích thước lớn hơn để đảm bảo bao trọn
            targetCamera.orthographicSize = Mathf.Max(targetSizeByHeight, targetSizeByWidth);
        }
    }
}