namespace World.Card.Skill
{
    public class SkillCounterTemplateModel: SkillEffectTemplateModel
    {
        public short Rate;
        /// <summary>
        /// Nếu rate success thì damage counter sẽ tính từ công thức ra
        /// </summary>
        public string Formular;
    }
}