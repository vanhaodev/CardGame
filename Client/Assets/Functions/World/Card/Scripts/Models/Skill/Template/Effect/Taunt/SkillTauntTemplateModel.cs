using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillTaunt", menuName = "Data/Skill/Effect/SkillTaunt")]
    public class SkillTauntTemplateModel : SkillEffectTemplateModel
    {
        public short Rate;
    }
}