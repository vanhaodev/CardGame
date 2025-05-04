using System.Threading.Tasks;
using UnityEngine;
using Unity.Cinemachine;

namespace Utils
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CinemachineCameraShake : MonoBehaviour
    {
        private CinemachineBasicMultiChannelPerlin _noise;

        void Awake()
        {
            var cinemachineCam = GetComponent<CinemachineCamera>();
            _noise = cinemachineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public async void Shake(float amplitude, float frequency, float duration)
        {
            if (_noise == null) return;

            _noise.AmplitudeGain = amplitude;
            _noise.FrequencyGain = frequency;

            await Task.Delay((int)(duration * 1000));

            _noise.AmplitudeGain = 0;
            _noise.FrequencyGain = 0;
        }
    }
}