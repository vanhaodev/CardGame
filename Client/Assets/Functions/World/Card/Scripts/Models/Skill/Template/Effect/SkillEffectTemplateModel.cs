using System.Collections.Generic;
using UnityEngine;
using World.Requirement;

namespace World.Card.Skill
{
    [System.Serializable]
    /// <summary>
    /// Mỗi level của một skill sẽ có option khác biệt mạnh dần
    /// </summary>
    public abstract class SkillEffectTemplateModel : ScriptableObject
    {
        /// <summary>
        /// Effect chỉ kích hoạt nếu caster tác động lên đúng loại mục tiêu
        /// </summary>
        public SkillEffectTriggerModel Trigger;
    }
}