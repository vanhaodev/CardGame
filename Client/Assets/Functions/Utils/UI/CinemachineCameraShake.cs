using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Cinemachine;

namespace Utils
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CinemachineCameraShake : MonoBehaviour, IGlobal
    {
        private CinemachineBasicMultiChannelPerlin _noise;
        private CancellationTokenSource _cinematicMotionCts;

        void Awake()
        {
            // Có thể Init() sau
        }

        public async UniTask Init()
        {
            var cinemachineCam = GetComponent<CinemachineCamera>();
            _noise = cinemachineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        [Button]
        public async void Shake(float amplitude = 2, float frequency = 3, float duration = 0.2f)
        {
            if (_noise == null) return;

            _noise.AmplitudeGain = amplitude;
            _noise.FrequencyGain = frequency;

            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            _noise.AmplitudeGain = 0;
            _noise.FrequencyGain = 0;
        }

        /// <summary>
        /// Bắt đầu hiệu ứng chuyển động điện ảnh. Nếu đang chạy thì tự override.
        /// </summary>
        [Button]
        public async void StartCinematicMotionEffect(
            float minAmp = 0.05f,
            float maxAmp = 0.2f,
            float minFreq = 0.1f,
            float maxFreq = 0.5f,
            float interval = 0.3f)
        {
            if (_noise == null) return;

            // Hủy loop cũ nếu có
            _cinematicMotionCts?.Cancel();
            _cinematicMotionCts = new CancellationTokenSource();
            var token = _cinematicMotionCts.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    _noise.AmplitudeGain = UnityEngine.Random.Range(minAmp, maxAmp);
                    _noise.FrequencyGain = UnityEngine.Random.Range(minFreq, maxFreq);

                    await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                // Bị hủy, reset noise
                _noise.AmplitudeGain = 0;
                _noise.FrequencyGain = 0;
            }
        }

        /// <summary>
        /// Dừng hiệu ứng chuyển động điện ảnh.
        /// </summary>
        [Button]
        public void StopCinematicMotionEffect()
        {
            _cinematicMotionCts?.Cancel();
            _cinematicMotionCts = null;

            if (_noise != null)
            {
                _noise.AmplitudeGain = 0;
                _noise.FrequencyGain = 0;
            }
        }
    }
}