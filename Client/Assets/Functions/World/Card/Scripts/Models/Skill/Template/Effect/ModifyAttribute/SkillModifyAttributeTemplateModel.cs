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
        /// <summary>
        /// R_Attack * 0.7: tăng lấy 70% attack của người nhận cộng cho người nhận <br/>
        /// S_Attack * 0.7: tăng lấy 70% attack của thi triển cộng cho người nhận <br/>
        /// </summary>
        public string Formula = "R_Attack * 0.7";
    }
}