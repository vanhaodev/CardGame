using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillModifyAttributeTemplateModel: SkillEffectTemplateModel
    {
        /// <summary>
        /// Số round ảnh hưởng
        /// </summary>
        public byte Round;
        /// <summary>
        /// Loại attribute muốn thay đổi
        /// </summary>
        public AttributeType ModifyAttributeType;
        public string Formula;
    }
}