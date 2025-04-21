namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillAttributeBonusModel
    {
        public AttributeType AttributeType;

        /// <summary>
        /// điểm thực
        /// </summary>
        public int Value;

        /// <summary>
        /// cộng từ base của card
        /// </summary>
        public int PercentBaseValue;
    }
}