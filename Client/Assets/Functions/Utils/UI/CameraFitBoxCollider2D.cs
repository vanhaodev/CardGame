using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Utils
{
    public class CameraFitBoxCollider2D : MonoBehaviour
    {
        public Camera targetCamera; // Main Camera thường
        public CinemachineCamera targetCamera2; // Cinemachine Camera (ưu tiên nếu có)
        public BoxCollider2D targetCollider; // Collider cần bao trọn
        public float padding = -1f; // Thêm khoảng trống tránh bị cắt

        private void Start()
        {
            Init();
        }

        async void Init()
        {
            await Task.Yield();
            if (targetCollider == null) return;

            // Kiểm tra xem có sử dụng Cinemachine không
            bool useCinemachine = targetCamera2 != null;
            float orthoSize;

            // Lấy tọa độ biên của collider
            Bounds bounds = targetCollider.bounds;
            float colliderWidth = bounds.size.x;
            float colliderHeight = bounds.size.y;

            // Aspect ratio của camera
            float screenRatio = (float)Screen.width / Screen.height;
            float targetSizeByHeight = colliderHeight / 2f + padding;
            float targetSizeByWidth = (colliderWidth / 2f + padding) / screenRatio;

            // Chọn kích thước lớn hơn để đảm bảo bao trọn
            orthoSize = Mathf.Max(targetSizeByHeight, targetSizeByWidth);

            if (useCinemachine)
            {
                // Nếu dùng Cinemachine, chỉnh kích thước trên CinemachineCamera
                targetCamera2.Lens.OrthographicSize = orthoSize;
            }
            else if (targetCamera != null)
            {
                // Nếu không dùng Cinemachine, chỉnh trực tiếp trên Main Camera
                targetCamera.orthographicSize = orthoSize;
            }
        }
    }
}