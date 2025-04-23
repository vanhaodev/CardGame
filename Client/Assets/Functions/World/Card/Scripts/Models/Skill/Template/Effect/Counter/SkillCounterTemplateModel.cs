using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]

    public class SkillCounterTemplateModel: SkillEffectTemplateModel
    {
        public short Rate;
        /// <summary>
        /// Nếu rate success thì damage counter sẽ tính từ công thức ra
        /// </summary>
        public string Formula;
    }
}