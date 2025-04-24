using System.Collections.Generic;

namespace World.TheCard.Skill
{
    public class SkillElementTriggerModel : SkillEffectTriggerModel
    {
        public List<ElementType> ElementTypes;
        public override bool IsSatisfied(Card sender, Card receiver)
        {
            throw new System.NotImplementedException();
        }
    }
}