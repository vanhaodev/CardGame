using System.Collections.Generic;
using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillOptionTemplateModel
    {
        /// <summary>
        /// cần round để hồi nếu kích hoạt xong
        /// </summary>
        public int CooldownRound;
        /// <summary>
        /// Khi sở hữu skill sẽ được tăng vĩnh viễn các chỉ số
        /// </summary>
        public List<SkillAttributeBonusModel> SkillAttributeBonus;
        /// <summary>
        /// Hiệu ứng kỹ năng
        /// </summary>
        [SerializeReference] public List<SkillEffectTemplateModel> SkillEffects;
        /// <summary>
        /// Hiệu ứng hình ảnh
        /// </summary>
        public List<SkillVisualEffectModel> SkillVisualEffects;
    }
}