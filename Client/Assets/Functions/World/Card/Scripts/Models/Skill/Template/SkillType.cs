namespace World.TheCard.Skill
{
    /// <summary>
    /// Skilltype quyết định các mà skill đó hoạt động <br/>
    /// ex: skill chúc phúc mùa xuân có khả năng Team => sẽ chỉ được chọn mục tiêu là đội, không được self <br/>
    /// ex: skill buff bức tường băng giá có khả năng Team, Sefl và Enemy => nếu buff cho phe thì phe nhận giáp băng, debuff cho địch thì địch lạnh quá nên giảm thủ <br/>
    /// PS: có thể chọn nhiều type cùng lúc nhé
    /// </summary>
    public enum SkillType
    {
        /// <summary>
        /// bị động, tự kích hoạt
        /// </summary>
        Passive,

        /// <summary>
        /// Chủ động cần kích hoạt thủ công
        /// </summary>
        Active
    }
}