namespace World.Card
{
    public enum SkillTriggerType : byte
    {
        /// <summary>
        /// Chủ động thực hiện
        /// </summary>
        Active,
        /// <summary>
        /// Nội tại tăng vĩnh viễn
        /// </summary>
        Passive,
        /// <summary>
        /// khích hoạt khi bị tấn công, thực hiện lên kẻ tấn công hoặc bản thân hoặc đội
        /// </summary>
        Counter,
        // /// <summary>
        // /// Sau khi hành động, nếu đạt một điều kiện sẽ kích hoạt
        // /// </summary>
        // FollowUp, 
        // /// <summary>
        // /// Tự thực hiện mỗi round
        // /// </summary>
        // Auto
    }
}