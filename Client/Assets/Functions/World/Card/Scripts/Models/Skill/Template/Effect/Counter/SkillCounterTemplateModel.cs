using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillCounter", menuName = "Data/Skill/Effect/SkillCounter")]

    public class SkillCounterTemplateModel: SkillEffectTemplateModel
    {
        public short Rate;
        /// <summary>
        /// Nếu rate success thì damage counter sẽ tính từ công thức ra
        /// </summary>
        public string Formula;
    }
}