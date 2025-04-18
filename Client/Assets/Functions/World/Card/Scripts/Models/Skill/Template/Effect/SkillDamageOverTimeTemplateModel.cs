using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillDamageOverTime", menuName = "Data/Skill/Effect/SkillDamageOverTime")]

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