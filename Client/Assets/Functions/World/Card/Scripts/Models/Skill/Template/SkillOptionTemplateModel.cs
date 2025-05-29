using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillOptionTemplateModel
    {
        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToLeftCount là 2 thì card số 2 và 3 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        public byte AOEToLeftTargetCount;

        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToRightCount là 1 thì card số 5 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        public byte AOEToRightTargetCount;

        /// <summary>
        /// cần round để hồi nếu kích hoạt xong
        /// </summary>
        public int CooldownRound;

        /// <summary>
        /// Khi sở hữu skill sẽ được tăng vĩnh viễn các chỉ số
        /// </summary>
        public List<SkillAttributeBonusModel> SkillAttributeBonus;

        /// <summary>
        /// Hiệu ứng kỹ năng
        /// </summary>
        [SerializeReference] public List<SkillEffectTemplateModel> SkillEffects;

        /// <summary>
        /// Vì passive không bắt người chơi chọn target nên target sẽ dựa vào trigger để quyết định, tất nhiên các SkillEffects vẫn có điều kiện trigger của riêng nó hoặc không
        /// </summary>
        [BoxGroup("Only passive skill")] public List<SkillEffectTriggerModel> PassiveTriggers;
        
        // /// <summary>
        // /// Hiệu ứng hình ảnh
        // /// </summary>
        // public List<SkillVisualEffectModel> SkillVisualEffects;
    }
}