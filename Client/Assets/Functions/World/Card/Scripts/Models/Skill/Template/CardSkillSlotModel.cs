using System;

namespace World.Card.Skill
{
    [Serializable]
    public class CardSkillSlotModel
    {
        public CardSkillSlotType SlotType;
        public SkillTemplateModel Skill;
    }
}