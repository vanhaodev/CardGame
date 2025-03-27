using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace World.Card
{
    public class SkillBehaviorWaitModel: SkillBehaviorActionModel
    {
        public float TimeSecond;
        public override async UniTask Execute(Card actor, List<Card> targets)
        {
            await UniTask.WaitForSeconds(TimeSecond);
        }
    }
}