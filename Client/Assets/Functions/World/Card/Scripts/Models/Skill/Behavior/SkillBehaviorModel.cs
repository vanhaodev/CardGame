using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace World.Card
{
    public class SkillBehaviorModel
    {
        public int SkillId { get; set; }
        public int SkillUpgradeLevel { get; set; }
        public List<SkillBehaviorActionModel> Actions { get; set; } = new();

        public void Init(int skillId, int skillUpgradeLevel)
        {
            SkillId = skillId;
            SkillUpgradeLevel = skillUpgradeLevel;

            foreach (var ac in Actions)
            {
                if (ac.DotweenProgressActions != null)
                {
                    ac.DotweenProgressActions = ac.DotweenProgressActions.OrderBy(i => i.Progress).ToList();
                }
            }
        }

        public async UniTask Execute(Card actor, List<Card> targets)
        {
            foreach (var action in Actions)
            {
                if (action.ProcessTargetsSequentially)
                {
                    foreach (var target in targets)
                    {
                        if (action.Await)
                            await action.Execute(actor, target);
                        else
                            _ = action.Execute(actor, target);
                    }
                }
            }
        }
    }
}