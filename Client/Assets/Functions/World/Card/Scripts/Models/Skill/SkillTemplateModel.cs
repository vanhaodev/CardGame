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
        /// max 5 stars
        /// </summary>
        public List<SkillTemplateLevelModel> Levels;
    }
}