using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace World.Card
{
    public class SkillBehaviorMeleeModel : SkillBehaviorActionModel
    {
        /// <summary>
        /// Khi bắn có tóe lửa
        /// </summary>
        public string CastName { get; set; } = "";

        /// <summary>
        /// Khi bắn trúng mục tiêu có tóe máu
        /// </summary>
        public string HitName { get; set; } = "";

        public override async UniTask Execute(Card actor, Card target)
        {
            _tweenAction = actor.transform
                .DORotate(
                    new Vector3(0, 0, GetOffsetY(actor.transform.position, target.transform.position) < 0 ? 80 : -80),
                    0.2f)
                .OnUpdate(() => HandleDotweenProgressActions(actor, target));

            await _tweenAction.AsyncWaitForCompletion();
        }
    }
}