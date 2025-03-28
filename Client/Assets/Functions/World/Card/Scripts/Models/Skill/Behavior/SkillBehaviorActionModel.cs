using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace World.Card
{
    public class SkillBehaviorProgressActionModel
    {
        public List<SkillBehaviorActionModel> Actions { get; set; } = new();
        public float Progress { get; set; }

        /// <summary>
        /// để check gọi duy nhất 1 lần
        /// </summary>
        public bool Completed { get; set; }
    }

    public abstract class SkillBehaviorActionModel
    {
        public bool Await { get; set; }
        public bool ProcessTargetsSequentially { get; set; }

        protected Tween _tweenAction;

        /// <summary>
        /// Gọi khi Excute đạt tiến độ phù hợp, hiện tại chỉ dành cho class có DOTWEEN
        /// </summary>
        public List<SkillBehaviorProgressActionModel> DotweenProgressActions;

        public abstract UniTask Execute(Card actor, Card target);

        protected void HandleDotweenProgressActions(Card actor, Card target)
        {
            float elapsed = _tweenAction.ElapsedPercentage();

            foreach (var actionModel in DotweenProgressActions)
            {
                if (!actionModel.Completed && elapsed >= actionModel.Progress)
                {
                    actionModel.Completed = true; // Đánh dấu đã thực thi

                    // Chạy Actions trong Background để không làm chậm OnUpdate
                    UniTask.Void(async () =>
                    {
                        foreach (var ac in actionModel.Actions)
                        {
                            if (ac.Await)
                                await ac.Execute(actor, target);
                            else
                                _ = ac.Execute(actor, target);
                        }
                    });
                }
            }
        }

        public float GetOffsetY(Vector3 actorPosition, Vector3 targetPosition)
        {
            float offsetY = actorPosition.y > targetPosition.y ? 1.0f : -1.0f;
            return offsetY;
        }
    }
}