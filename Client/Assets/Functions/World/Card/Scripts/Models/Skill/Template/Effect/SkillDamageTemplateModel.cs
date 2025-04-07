using System.Collections.Generic;

namespace World.Card.Skill
{
    public class SkillDamageTemplateModel: SkillEffectTemplateModel
    {
        /// <summary>
        /// bonus của skill
        /// </summary>
        public int Damage;
        /// <summary>
        /// lấy điểm atk của nhân vật
        /// </summary>
        public short DamageAtkPercent;
    }
}