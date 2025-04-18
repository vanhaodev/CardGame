using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillConfuse", menuName = "Data/Skill/Effect/SkillConfuse")]

    public class SkillConfuseTemplateModel : SkillEffectTemplateModel
    {
        /// <summary>
        /// tỉ lệ thành công
        /// </summary>
        public short Rate;
        /// <summary>
        /// Có khả năng chệch phe, ví dụ attack có thể tấn công dính đồng đội, nếu buff có thể chệch sang địch
        /// </summary>
        public bool IsConfuseTargetFaction;
    }
}