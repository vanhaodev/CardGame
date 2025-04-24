using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]

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