using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Effects;
using Globals;

namespace World.TheCard
{
    public class CardEffect : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] CanvasGroup _canvasGroup;
        private CancellationTokenSource _touchCancellation; // Cancellation riêng cho hiệu ứng touch
        public async void PlaySpawn()
        {
            Global.Instance.Get<EffectManager>().ShowDeath(gameObject.transform.position).Forget();
            await _canvasGroup.DOFade(1, 1f)
                // .OnPlay()
                .AsyncWaitForCompletion();
        }
        public async void PlayDie()
        {
            Global.Instance.Get<EffectManager>().ShowDeath(gameObject.transform.position).Forget();
            await _canvasGroup.DOFade(0, 1f)
                // .OnPlay()
                .AsyncWaitForCompletion();
        }
    }
}