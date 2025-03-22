namespace World.Card
{
    public enum ClassType : byte
    {
        None = 0, // Không có class (mặc định)

        Juggernaut = 1, // "Chiến Thần" - Sát thương cận chiến mạnh, công thủ toàn diện (The Destruction)
        Assassin = 2, // "Sát Thủ" - Sát thương đơn cực mạnh, ưu tiên mục tiêu quan trọng (The Hunt)
        Demolisher = 3, // "Phá Hoại" - Sát thương diện rộng, sử dụng vũ khí hạng nặng (The Erudition)
        Tactician = 4, // "Chiến Lược Gia" - Buff tấn công, phòng thủ cho đồng đội (The Harmony)
        Saboteur = 5, // "Phá Rối" - Gây hiệu ứng bất lợi, vô hiệu hóa kẻ địch (The Nihility)
        Guardian = 6, // "Hộ Vệ" - Tanker, tạo khiên bảo vệ đồng đội (The Preservation)
        Medic = 7 // "Bác Sĩ Quân Sự" - Hồi máu, cứu trợ đồng đội (The Abundance)
    }
}