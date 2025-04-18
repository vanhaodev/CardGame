using System.Collections.Generic;
using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillDamage", menuName = "Data/Skill/Effect/SkillDamage")]

    public class SkillDamageTemplateModel: SkillEffectTemplateModel
    {
        public string Formula;
    }
}