using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillDelayRound", menuName = "Data/Skill/Effect/SkillDelayRound")]

    public class SkillDelayRoundTemplateModel: SkillEffectTemplateModel
    {
        public byte Round;
    }
}