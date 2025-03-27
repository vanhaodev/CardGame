using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace World.Card
{
    public class SkillBehaviorModel
    {
        public List<SkillBehaviorActionModel> Actions { get; set; } = new();

        public async UniTask Execute(Card actor, List<Card> targets)
        {
            foreach (var action in Actions)
            {
                if (action.ProcessTargetsSequentially)
                {
                    foreach (var target in targets)
                    {
                        if (action.Await)
                            await action.Execute(actor, new List<Card> { target });
                        else
                            _ = action.Execute(actor, new List<Card> { target });
                    }
                }
                else
                {
                    if (action.Await)
                        await action.Execute(actor, targets);
                    else
                        _ = action.Execute(actor, targets);
                }
            }
        }
    }
}