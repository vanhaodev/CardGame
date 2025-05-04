using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using World.TheCard.Skill;
using World.Requirement;

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
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToLeftCount là 2 thì card số 2 và 3 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        [BoxGroup("Base")] [GUIColor(1f, 1f, 0f)]
        public byte AOEToLeftTargetCount;

        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToRightCount là 1 thì card số 5 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        [BoxGroup("Base")] [GUIColor(1f, 1f, 0f)]
        public byte AOEToRightTargetCount;

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