using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace World.Card
{
    public class SkillBehaviorMovePosModel : SkillBehaviorMoveForwardModel
    {
        public Vector3 Target { get; set; }
        public override async UniTask Execute(Card actor, Card target)
        {
            Vector3 attackPosition = Target;
            _tweenAction = actor.transform
                .DOMove(attackPosition, MoveTime)
                .OnUpdate(() => HandleDotweenProgressActions(actor, target));

            await _tweenAction.AsyncWaitForCompletion();
        }
    }
}