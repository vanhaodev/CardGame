using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillModifyAttribute", menuName = "Data/Skill/Effect/SkillModifyAttribute")]
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