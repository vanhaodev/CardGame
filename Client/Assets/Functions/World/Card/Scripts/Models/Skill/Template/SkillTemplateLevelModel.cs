using System.Collections.Generic;
using World.Requirement;

namespace World.Card
{
    /// <summary>
    /// Mỗi level của một skill sẽ có option khác biệt mạnh dần
    /// </summary>
    public class SkillTemplateLevelModel
    {
        /// <summary>
        /// Học hoặc nâng cấp cần thỏa điều kiện, nếu là currency hoặc item thì sẽ bị trừ tiền hoặc item
        /// </summary>
        public RequirementModel UpgradeRequirement;
        public SkillTemplateActiveModel Active;
        public SkillTemplatePassiveModel Passive;
        public SkillTemplateCounterModel Counter;
    }
}