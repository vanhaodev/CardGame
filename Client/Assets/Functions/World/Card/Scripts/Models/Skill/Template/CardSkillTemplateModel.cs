using System;

namespace World.TheCard.Skill
{
    [Serializable]
    public class CardSkillTemplateModel
    {
        public CardSkillSlotType SlotType;
        public SkillTemplateModel Skill;
    }
}