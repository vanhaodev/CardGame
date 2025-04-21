using System.Collections.Generic;

namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillOptionTemplateModel
    {
        /// <summary>
        /// Khi sở hữu skill sẽ được tăng vĩnh viễn các chỉ số
        /// </summary>
        public List<SkillAttributeBonusModel> SkillAttributeBonus;
        /// <summary>
        /// Hiệu ứng kỹ năng
        /// </summary>
        public List<SkillEffectTemplateModel> SkillEffects;
        /// <summary>
        /// Hiệu ứng hình ảnh
        /// </summary>
        public List<SkillVisualEffectModel> SkillVisualEffects;
    }
}