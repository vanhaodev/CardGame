using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FloatingEffect;
using Globals;
using UnityEngine;

namespace World.Card
{
    public class SkillBehaviorRangedModel : SkillBehaviorMeleeModel
    {
        /// <summary>
        /// Vfx của viên đạn
        /// </summary>
        public string BulletName { get; set; } = "FloatingEffect/FloatingEffectBullet.prefab";

        public float MoveTime { get; set; }

        public override async UniTask Execute(Card actor, Card target)
        {
            var floatingEffectManager = Global.Instance.Get<FloatingEffectManager>();
            string prefabAddress = BulletName;
            FloatingEffectParticle effect =
                await floatingEffectManager.GetEffectFromPoolOrCreate(prefabAddress) as FloatingEffectParticle;
            Transform effectTransform = effect.transform;
            effectTransform.position = actor.transform.position;
            _tweenAction = effectTransform.DOMove(target.transform.position, MoveTime).SetEase(Ease.Linear)
                .OnUpdate(() => HandleDotweenProgressActions(actor, target));
            await _tweenAction.AsyncWaitForCompletion();
            floatingEffectManager.ReleaseEffect(effect);
        }
    }
}