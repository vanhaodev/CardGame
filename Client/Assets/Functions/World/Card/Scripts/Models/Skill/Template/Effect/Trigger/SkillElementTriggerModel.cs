using System.Collections.Generic;
using GameConfigs;
using Globals;

namespace World.TheCard.Skill
{
    /// <summary>
    /// Kiểm tra nếu mục tiêu muốn check có element phù hợp sẽ kích hoạt
    /// </summary>
    public class SkillElementTriggerModel : SkillEffectTriggerModel
    {
        public List<ElementType> ElementTypes;

        public override bool IsSatisfied(Card sender, Card receiver)
        {
            var checks = GetCheckableCards(sender, receiver);
            foreach (var card in checks)
            {
                if (ElementTypes.Contains(card.Battle.ElementType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}