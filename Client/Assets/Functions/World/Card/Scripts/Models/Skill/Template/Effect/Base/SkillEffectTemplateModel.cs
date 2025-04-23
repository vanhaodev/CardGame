using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using World.Card.Skill;
using World.Requirement;

namespace World.Card.Skill
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
        [SerializeReference] public List<SkillEffectTriggerModel> Triggers;

        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToLeftCount là 2 thì card số 2 và 3 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        public byte AOEToLeftTargetCount;

        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToRightCount là 1 thì card số 5 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        public byte AOEToRightTargetCount;
    }
}