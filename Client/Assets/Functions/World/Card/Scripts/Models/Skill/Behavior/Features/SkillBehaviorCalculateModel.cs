using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace World.Card
{
    public class SkillBehaviorCalculateModel : SkillBehaviorActionModel
    {
        public async override UniTask Execute(Card actor, Card target)
        {
            var attackerDamage = actor.Battle.GetDamage();
            target.Battle.OnTakeDamage(attackerDamage.damage, actor);
            target.Battle.OnTakeDamageLate();
        }
    }
}