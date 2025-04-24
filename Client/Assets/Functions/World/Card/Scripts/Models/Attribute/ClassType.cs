namespace World.TheCard
{
    public enum ClassType : byte
    {
        None = 0, // Không có class (mặc định)

        /// <summary>
        /// "Chiến Thần" - Sát thương cận chiến mạnh, công thủ toàn diện (The Destruction) <br/>
        /// Hướng tới DPS cận chiến mạnh, tăng ATK và HP khá cao, đồng thời có một chút cải thiện về DEF và Critical Chance. Sát thương Break cũng cao để phá vỡ mục tiêu nhanh chóng.
        /// </summary>
        Juggernaut = 1,
        /// <summary>
        /// "Sát Thủ" - Sát thương đơn cực mạnh, ưu tiên mục tiêu quan trọng (The Hunt) <br/>
        /// ATK rất cao để tối ưu hóa sát thương đơn mục tiêu. Chỉ số Speed cao giúp hành động trước và có khả năng Critical Damage mạnh. Tuy nhiên, DEF không cao.
        /// </summary>
        Assassin = 2, 
        /// <summary>
        /// "Phá Hoại" - Sát thương diện rộng, sử dụng vũ khí hạng nặng (The Erudition) <br/>
        /// Sát thương diện rộng là thế mạnh, do đó, HP và ATK tăng đều. Cải thiện Break Damage để gây thiệt hại mạnh mẽ khi phá vỡ mục tiêu.
        /// </summary>
        Demolisher = 3,
        /// <summary>
        /// "Chiến Lược Gia" - Buff tấn công, phòng thủ cho đồng đội (The Harmony) <br/>
        /// Tăng cường chỉ số Buff và hỗ trợ cho đồng đội. Các chỉ số như Effect Hit Rate và Energy Regeneration sẽ tăng mạnh. ATK và DEF đều được cải thiện nhẹ để hỗ trợ.
        /// </summary>
        Tactician = 4, 
        /// <summary>
        /// "Hộ Vệ" - Tanker, tạo khiên bảo vệ đồng đội (The Preservation) <br/>
        /// Tanker, tăng DEF và HP để bảo vệ đồng đội. Chỉ số Speed thấp hơn để không chiếm quá nhiều ưu thế về hành động, nhưng có thể bảo vệ đồng đội tốt.
        /// </summary>
        Guardian = 5,
        /// <summary>
        /// "Bác Sĩ Quân Sự" - Hồi máu, cứu trợ đồng đội (The Abundance) <br/>
        /// Tăng HP và Energy Regeneration cao để hỗ trợ hồi máu cho đồng đội. Outgoing Healing là chỉ số đặc biệt, tăng mạnh để tối ưu hóa khả năng hồi phục.
        /// </summary>
        Medic = 6 
    }
}