using System.Collections.Generic;

namespace World.TheCard.Skill
{
    public class SkillAttributeTriggerModel : SkillEffectTriggerModel
    {
        public List<AttributeModel> Attributes;
        public List<AttributeModel> AttributePercents;
        public override bool IsSatisfied(Card sender, Card receiver)
        {
            throw new System.NotImplementedException();
        }
    }
}