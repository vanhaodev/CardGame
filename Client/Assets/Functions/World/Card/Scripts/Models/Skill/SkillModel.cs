using System.Collections.Generic;

namespace World.Card
{
    public class SkillModel
    {
        public ushort TemplateId;
        public byte UpgradeLevel;
        public SkillBattleModel Battle;
    }

    public class SkillBattleModel
    {
        public byte CooldownRoundRemaining;

        public List<SkillTemplateRequirementType> CheckRequirement()
        {
            return new List<SkillTemplateRequirementType>();
        }

        public bool IsReady()
        {
            return CooldownRoundRemaining <= 0 && new List<SkillTemplateRequirementType>().Count == 0;
        }
    }
}