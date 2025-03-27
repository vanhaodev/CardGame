using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace World.Card
{
    public class SkillBehaviorAnimationModel : SkillBehaviorActionModel
    {
        public string AnimationName { get; set; } = "";

        public override async UniTask Execute(Card actor, List<Card> targets)
        {
            // Gọi animation
            await UniTask.Delay(1000); // Giả lập thời gian chạy animation
        }
    }
}