namespace World.TheCard
{
    public enum ElementType
    {
        /// <summary>
        /// Kim – Sắt thép, chính xác, xuyên phá <br/>
        /// ✅ Mạnh: Xuyên giáp, chí mạng, kháng hiệu ứng <br/>
        /// ❌ Yếu: Máu thấp, dễ bị phá vỡ Toughness <br/>
        /// ⚔️ Phong cách: Đòn đánh chính xác, xuyên thủ, tối ưu debuff nhanh
        /// </summary>
        Metal = 1,

        /// <summary>
        /// Mộc – Linh hoạt, phản xạ, phát triển <br/>
        /// ✅ Mạnh: Né tránh, hồi phục nhẹ, tốc độ cao <br/>
        /// ❌ Yếu: Sát thương thấp, khó bùng nổ <br/>
        /// ⚔️ Phong cách: Né đòn, phản công, tăng dần theo thời gian
        /// </summary>
        Wood = 2,

        /// <summary>
        /// Thủy – Kiểm soát, làm chậm, phá nhịp <br/>
        /// ✅ Mạnh: Hồi năng lượng (UP), làm chậm, gây debuff <br/>
        /// ❌ Yếu: Phòng thủ yếu, phụ thuộc kỹ năng <br/>
        /// ⚔️ Phong cách: Điều khiển trận đấu bằng debuff và combo kỹ năng
        /// </summary>
        Water = 3,

        /// <summary>
        /// Hỏa – Bạo phát, thiêu đốt, đột phá <br/>
        /// ✅ Mạnh: Sát thương cao, chí mạng, gây bỏng/hiệu ứng theo thời gian <br/>
        /// ❌ Yếu: Khó kiểm soát, dễ bị khống chế <br/>
        /// ⚔️ Phong cách: Dồn dame, gây sát thương lan, thiên về áp lực sớm
        /// </summary>
        Fire = 4,

        /// <summary>
        /// Thổ – Bền bỉ, phòng ngự, chống chịu <br/>
        /// ✅ Mạnh: Máu cao, khiên, kháng sát thương, chống chịu hiệu ứng <br/>
        /// ❌ Yếu: Tốc độ chậm, ít hiệu ứng chủ động <br/>
        /// ⚔️ Phong cách: Kiên trì, phòng thủ, phản đòn và kéo dài trận
        /// </summary>
        Earth = 5
    }

}