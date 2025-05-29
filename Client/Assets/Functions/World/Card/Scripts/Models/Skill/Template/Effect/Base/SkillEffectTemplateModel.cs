using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using World.TheCard.Skill;

namespace World.TheCard.Skill
{
    [System.Serializable]
    /// <summary>
    /// Mỗi level của một skill sẽ có option khác biệt mạnh dần
    /// </summary>
    public abstract class SkillEffectTemplateModel
    {
        /// <summary>
        /// Effect chỉ kích hoạt nếu caster tác động lên đúng loại mục tiêu, ví dụ hiệu ứng trói buộc sẽ gây nên mục tiêu đang có UP thấp hơn 10%
        /// </summary>
        [BoxGroup("Base")] [GUIColor(1f, 1f, 0f)] [SerializeReference]
        public List<SkillEffectTriggerModel> Triggers;

        /// <summary>
        /// số round ảnh hưởng
        /// </summary>
        [BoxGroup("Base")] [GUIColor(1f, 1f, 0f)]
        public byte ImpactRound = 1;

        /// <summary>
        /// tỉ lệ trúng
        /// </summary>
        [BoxGroup("Base")] [GUIColor(1f, 1f, 0f)]
        public short SuccessRate = 10000;
    }
}