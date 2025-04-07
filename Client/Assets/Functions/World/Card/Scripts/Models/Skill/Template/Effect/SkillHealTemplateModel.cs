namespace World.Card.Skill
{
    public class SkillHealTemplateModel : SkillEffectTemplateModel
    {
        //Heal sau khi tính toán sẽ được nhân với Ongoing Healing để tăng hiệu quả
        public Model Hp;
        public Model Shield;
        public Model SP;
        public Model UP;

        public class Model
        {
            /// <summary>
            /// điểm thực
            /// </summary>
            public int? Real;
            /// <summary>
            /// Lấy điểm tấn công
            /// </summary>
            public short? AtkPercent;
            /// <summary>
            /// % của điểm đang có
            /// </summary>
            public int? CurrentPercent;
            /// <summary>
            /// % của điểm max
            /// </summary>
            public int? MaxPercent;
        }
    }
}