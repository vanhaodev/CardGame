using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]

    public class SkillCounterTemplateModel: SkillEffectTemplateModel
    {
        /// <summary>
        /// Tỷ lệ phần trăm sát thương sẽ phản đòn lại kẻ tấn công nếu điều kiện phản đòn thành công.
        /// <br/>
        /// Ví dụ: Nếu sát thương nhận vào là 100 và CounterPercent = 0.5% (50%),
        /// thì sát thương phản đòn sẽ là 100 * 0.5 = 50.
        /// </summary>
        public float CounterPercent = 0.5f;
    }
}