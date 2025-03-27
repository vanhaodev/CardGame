using System.Collections.Generic;

namespace World.Card
{
    public enum SkillTemplateRequirementType
    {
        HPCost,
        MPCost,
        HP,
        MP,
        TeamAliveCount,
        TeamAndSelfAliveCount,
        EnemyAliveCount
    }

    public class SkillTemplateRequirementMaxMinModel
    {
        /// <summary>
        /// Khoảng min có thể kích hoạt
        /// </summary>
        public int Min;

        /// <summary>
        /// Khoảng max có thể kích hoạt
        /// </summary>
        public int Max;
    }

    public class SkillTemplateRequirementModel
    {
        public Dictionary<SkillTemplateRequirementType, SkillTemplateRequirementMaxMinModel> Requirements;
    }
}