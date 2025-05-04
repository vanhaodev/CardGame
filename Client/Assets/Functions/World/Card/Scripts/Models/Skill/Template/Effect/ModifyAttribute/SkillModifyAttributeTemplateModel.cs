using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillModifyAttributeTemplateModel: SkillEffectTemplateModel
    {
        /// <summary>
        /// Loại attribute muốn thay đổi
        /// </summary>
        public AttributeType ModifyAttributeType;
        public string Formula;
    }
}