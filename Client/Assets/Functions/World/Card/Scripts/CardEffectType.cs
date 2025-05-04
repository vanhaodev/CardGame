namespace World.TheCard
{
    /// <summary>
    /// Actor will play effect of sender and target will player effect of receiver
    /// </summary>
    public enum CardEffectType
    {
        Attack,
        Heal,
        Buff,
        Debuff,
        Die,
        /// <summary>
        /// Hồi sinh
        /// </summary>
        Revive
    }
}