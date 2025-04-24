using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]

    public class SkillDamageOverTimeTemplateModel: SkillDamageTemplateModel
    {
        /// <summary>
        /// damage chia cho số round
        /// </summary>
        public bool DamageDividedByRound;
        /// <summary>
        /// số round ảnh hưởng
        /// </summary>
        public byte Round;
    }
}