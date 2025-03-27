using System.Collections.Generic;

namespace World.Card
{
    public class SkillTemplateModel
    {
        public ushort Id;
        public string Name;
        public string Description;
        public SkillTriggerType TriggerType;
        /// <summary>
        /// Cấp của card, không phải cấp sao nha
        /// </summary>
        public SkillTemplateRequirementModel LevelRequirement;
        /// <summary>
        /// max 5 stars
        /// </summary>
        public List<SkillTemplateLevelModel> Levels;
    }
}