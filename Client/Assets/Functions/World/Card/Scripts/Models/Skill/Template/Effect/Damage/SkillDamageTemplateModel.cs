using System.Collections.Generic;
using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillDamageTemplateModel : SkillEffectTemplateModel
    {
        /// <summary>
        /// Công thức tính sát thương cuối cùng:
        /// Nhân phần trăm sức tấn công, cộng bonus sát thương, nhân với hệ số chí mạng (nếu có),
        /// sau đó trừ đi phòng thủ của mục tiêu.
        /// <br/>
        /// Nếu không chí mạng, S_CriticalDamage mặc định là 1 (không thay đổi sát thương).
        /// <br/>
        /// Ví dụ: S_Attack = 100, R_Defense = 10, S_CriticalDamage = 1.5 (crit success), 
        /// <br/>
        /// Damage = ((100 * 1.70) + 255) * 1.5 - 10 = 622.5
        /// </summary>
        public string Formula = "(((S_Attack * 1.70) + 255) * S_CriticalDamage) - R_Defense";
    }
}