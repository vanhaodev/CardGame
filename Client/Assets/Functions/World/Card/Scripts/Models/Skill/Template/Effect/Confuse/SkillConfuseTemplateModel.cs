using UnityEngine;
using UnityEngine.Serialization;

namespace World.TheCard.Skill
{
    /// <summary>
    /// Gây bối rối cho kẻ địch, đòn đánh có thể lệch sang mục tiêu khác <br/>
    /// Nếu có confuse phe thì lệch cả phe như đánh địch thì có khả năng lệch về đánh đội, buff đội có khả năng buff cho địch rất nguy hiểm.
    /// </summary>
    [System.Serializable]
    public class SkillConfuseTemplateModel : SkillEffectTemplateModel
    {
        /// <summary>
        /// Có khả năng chệch phe, ví dụ attack có thể tấn công dính đồng đội, nếu buff có thể chệch sang địch
        /// </summary>
        public short SwapFactionRate;
    }
}