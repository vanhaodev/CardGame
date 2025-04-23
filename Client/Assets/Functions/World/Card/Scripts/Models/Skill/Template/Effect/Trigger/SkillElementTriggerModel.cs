using System.Collections.Generic;

namespace World.Card.Skill
{
    public class SkillElementTriggerModel : SkillEffectTriggerModel
    {
        public List<ElementType> ElementTypes;
        public override bool IsSatisfied(Card target)
        {
            throw new System.NotImplementedException();
        }
    }
}