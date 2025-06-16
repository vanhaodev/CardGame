namespace Functions.World.Player.Inventory
{
    public enum EquipmentType
    {
        /// <summary>
        /// Vũ khí chính: Kiếm, đao, dao, súng, cung, rìu, gậy trượng, sách, đấm, vuốt, nỏ...
        /// Dùng để gây sát thương chính. Mỗi nhân vật chỉ đeo 1 vũ khí.
        /// </summary>
        Weapon = 1,

        /// <summary>
        /// Giáp bảo hộ: Gộp từ nón, áo, quần – tăng máu, phòng thủ, kháng hiệu ứng.
        /// </summary>
        Armor = 2,

        /// <summary>
        /// Găng tay: Tăng độ chính xác, tốc độ, sát thương phụ trợ.
        /// </summary>
        Glove = 3,

        /// <summary>
        /// Giày: Tăng tốc độ, né tránh, phản ứng nhanh.
        /// </summary>
        Boots = 4,

        /// <summary>
        /// Trang sức: Gộp từ nhẫn và dây chuyền – tăng thuộc tính phụ (crit, hút máu, kháng debuff...).
        /// </summary>
        Accessory = 5,

        /// <summary>
        /// Cổ vật: Buff đặc biệt, hiệu ứng chủ động/bị động mạnh. Tùy theo build nhân vật.
        /// </summary>
        Relic = 6
    }
}