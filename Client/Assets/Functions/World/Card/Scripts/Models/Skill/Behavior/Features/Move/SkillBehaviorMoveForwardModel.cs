using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Globals;
using UnityEngine;

namespace World.Card
{
    public class SkillBehaviorMoveForwardModel : SkillBehaviorActionModel
    {
        public float MoveTime { get; set; }

        public override async UniTask Execute(Card actor, Card target)
        {
            Vector3 attackPosition = new Vector3(target.transform.position.x,
                target.transform.position.y + GetOffsetY(actor.transform.position, target.transform.position),
                target.transform.position.z);
            _tweenAction = actor.transform
                .DOMove(attackPosition, MoveTime)
                .OnUpdate(() => HandleDotweenProgressActions(actor, target));
            await _tweenAction.AsyncWaitForCompletion();
        }
    }
}