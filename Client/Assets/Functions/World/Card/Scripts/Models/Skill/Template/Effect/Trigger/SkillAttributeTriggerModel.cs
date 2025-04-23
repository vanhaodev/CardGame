using System.Collections.Generic;

namespace World.Card.Skill
{
    public class SkillAttributeTriggerModel : SkillEffectTriggerModel
    {
        public List<AttributeModel> Attributes;
        public List<AttributeModel> AttributePercents;
        public override bool IsSatisfied(Card target)
        {
            throw new System.NotImplementedException();
        }
    }
}