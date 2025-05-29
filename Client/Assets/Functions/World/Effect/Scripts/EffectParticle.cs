using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Effects
{
    public class EffectParticle : Effect
    {
        private enum PlayType
        {
            Random,
            All,
            AllInOneTime,
            AllInRandomOrder
        }

        [SerializeField] private PlayType _playType;
        [SerializeField] private ParticleSystem[] _particleSystems;

        [Button]
        public async UniTask Play()
        {
            if (_particleSystems == null || _particleSystems.Length == 0) return;

            switch (_playType)
            {
                case PlayType.Random:
                    await PlayRandom();
                    break;
                case PlayType.All:
                    await PlayAllSequentially();
                    break;
                case PlayType.AllInOneTime:
                    await PlayAllSimultaneously();
                    break;
                case PlayType.AllInRandomOrder:
                    await PlayAllInRandomOrder();
                    break;
            }
        }

        private async UniTask PlayRandom()
        {
            var chosenOne = _particleSystems[UnityEngine.Random.Range(0, _particleSystems.Length)];
            if (chosenOne == null) return;

            chosenOne.Play();
            await UniTask.WaitUntil(() => !chosenOne.IsAlive(true));
        }

        private async UniTask PlayAllSequentially()
        {
            foreach (var particle in _particleSystems)
            {
                if (particle == null) continue;

                particle.Play();
                await UniTask.WaitUntil(() => !particle.IsAlive(true));
            }
        }

        private async UniTask PlayAllSimultaneously()
        {
            foreach (var particle in _particleSystems)
            {
                particle?.Play();
            }

            // Đợi tất cả particle kết thúc
            await UniTask.WaitUntil(() => _particleSystems.All(p => p == null || !p.IsAlive(true)));
        }

        private async UniTask PlayAllInRandomOrder()
        {
            List<ParticleSystem> shuffledList = _particleSystems.OrderBy(_ => UnityEngine.Random.value).ToList();

            foreach (var particle in shuffledList)
            {
                if (particle == null) continue;

                particle.Play();
                await UniTask.WaitUntil(() => !particle.IsAlive(true));
            }
        }
    }
}
