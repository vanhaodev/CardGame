using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FloatingEffect
{
    public class FloatingEffectParticle : FloatingEffect
    {
        [SerializeField] ParticleSystem[] _particleSystems;
        [Button]
        public async UniTask Play()
        {
            var choosenOne = _particleSystems[UnityEngine.Random.Range(0, _particleSystems.Length)];
            if (choosenOne == null) return;

            choosenOne.Play();
    
            await UniTask.DelayFrame(1); // Đợi 1 frame để chắc chắn particle bắt đầu chạy
            await UniTask.WaitUntil(() => !choosenOne.IsAlive(true)); // Chờ đến khi particle tắt
        }

    }
}