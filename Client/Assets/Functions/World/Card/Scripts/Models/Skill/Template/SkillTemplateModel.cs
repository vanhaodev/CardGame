using System.Collections.Generic;
using World.Requirement;

namespace World.Card.Skill
{
    public class SkillTemplateModel
    {
        public ushort Id;
        public string Name;
        public string Description;
        public SkillType SkillType;
        public SkillSlotType SkillSlotType;
        /// <summary>
        /// max 5 stars
        /// </summary>
        public List<SkillBuilderTemplateModel> Levels;
    }
}