namespace World.TheCard
{
    /// <summary>
    /// Các att % sẽ theo hệ ushort 10.000 = 100%
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// Máu tối đa
        /// </summary>
        HpMax,
        /// <summary>
        /// Tấn công, nếu tướng là hệ gì thì attack sẽ ra sát thương theo hệ đó nên chỉ cần attack là đủ, không cần pysic hay magic
        /// </summary>
        Attack,
        /// <summary>
        /// Phòng thủ bất kể hệ nào
        /// </summary>
        Defense,
        /// <summary>
        /// Tốc độ đánh, tốc càng cao thì càng nhiều lượt đánh trong trận turnbased
        /// </summary>
        Speed,
        /// <summary>
        /// Tỉ lệ chí mạng
        /// </summary>
        CriticalRate,
        /// <summary>
        /// Sát thương tăng thêm % khi đạt chí mạng
        /// </summary>
        CriticalDamage,
        /// <summary>
        /// Tỉ lệ né đòn
        /// </summary>
        DodgeRate,
        /// <summary>
        /// Né thành công sẽ giảm được % sát thương phải nhận
        /// </summary>
        DodgeDamage,
        /// <summary>
        /// Tỉ lệ xuyên giáp
        /// </summary>
        ArmorPenetrationChance,
        /// <summary>
        /// Xuyên giáp thành công sẽ giảm % thủ của mục tiêu để sát thương bớt bị hao hụt bởi phòng thủ
        /// </summary>
        ArmorPenetrationDamage,
        /// <summary>
        /// Tăng % khả năng gây hiệu ứng xấu lên kẻ địch.<br/>
        /// Nếu ta gây hiệu ứng debuff lên địch.<br/>
        /// nếu hiệu ứng đó có khả năng hụt thì chỉ số này sẽ giúp tăng % của % thành công (chiêu có 10% thành công nếu fx hit rate = 100% thì thành 20%)
        /// </summary>
        EffectHitRate,
        /// <summary>
        /// Giảm % khả năng bị dính hiệu ứng xấu.<br/>
        /// EffectHitRate tăng cho người thi triển còn EffectResistRate ngược lại, đấu đá nhau
        /// </summary>
        EffectResistRate,
        /// <summary>
        /// Tăng % tốc độ hồi năng lượng để dùng Ultimate.<br/>
        /// Mỗi lần tướng bị nhận sát thương thì sẽ hồi điểm ultimate, nếu UPR là 100% và hồi được 10 thì sẽ hồi được 20 điểm
        /// </summary>
        UPRegeneration,
        /// <summary>
        /// Tăng % lượng máu hồi được và lượng shield giáp tăng lên của kỹ năng có healing hoặc shield.<br/>
        /// OutgoingHealing = 10000 (100%), kỹ năng hồi 50 máu + OutgoingHealing => 100 máu
        /// </summary>
        OutgoingHealing,
        // Shield: là lượng giáp sẽ bị trừ thay cho máu đến khi hết giáp mới trừ máu, lượng giáp tối đa = hp max
        /// <summary>
        /// Tăng % lượng máu nhận được hoặc lượng shield giáp tăng lên khi được skill healing hoặc shield, chính mình heal và shield cho mình cũng được.<br/>
        /// Đồng đội có OutgoingHealing cao heal cho mình có HealingReceived cao thì lượng máu cực dồi dào.
        /// </summary>
        HealingReceived,
        /// <summary>
        /// Mỗi tướng có một thanh Toughness = 50% máu, khi bị hệ khắc mình đánh, sẽ trừ cả vào Toughness, khi Toughness về 0 sẽ xảy ra hiệu ứng phá<br/>
        /// vỡ và gây hiệu ứng đặc biệt của hệ đã đánh mình khiến Toughness = 0 (mỗi hệ sẽ gây ra hiệu ứng riêng)<br/>
        /// Sau khi hết hiệu ứng đặc biệt thì Toughness sẽ hồi lại 100%.
        /// </summary>
        ToughnessHitRate,
        /// <summary>
        /// Giảm sát thương kim nhận vào
        /// </summary>
        MetalResistant,
        /// <summary>
        /// Giảm sát thương mộc nhận vào
        /// </summary>
        WoodResistant,
        /// <summary>
        /// Giảm sát thương thuỷ nhận vào
        /// </summary>
        WaterResistant,
        /// <summary>
        /// Giảm sát thương hoả nhận vào
        /// </summary>
        FireResistant,
        /// <summary>
        /// Giảm sát thương thổ nhận vào
        /// </summary>
        EarthResistant,
        /// <summary>
        /// Giảm sát thương tất cả hệ nhận vào
        /// </summary>
        AllElementResistant,
        // Vì sao không cộng thẳng attack mà còn có chỉ số kiểu này? vì các skill buff<br/>
        // có thể tăng damage theo hệ cho đồng đội, tăng độ khó trong việc phù hợp đội hình
        /// <summary>
        /// Tăng sát thương kim
        /// </summary>
        MetalDamage,
        /// <summary>
        /// Tăng sát thương mộc
        /// </summary>
        WoodDamage,
        /// <summary>
        /// Tăng sát thương thuỷ
        /// </summary>
        WaterDamage,
        /// <summary>
        /// Tăng sát thương hoả
        /// </summary>
        FireDamage,
        /// <summary>
        /// Tăng sát thương thổ
        /// </summary>
        EarthDamage,
        /// <summary>
        /// Tăng sát thương cho all hệ
        /// </summary>
        AllElementDamage,
        /// <summary>
        /// Hút máu dựa theo sát thương gây ra cho địch<br/>
        /// ví dụ lifesteal 10% mà sát thương gây là 100 thì sẽ hút được 10 máu
        /// </summary>
        LifeSteal,
    }

    public enum BattleAttributeType
    {
        /// <summary>
        /// Hp hiện tại trong trận
        /// </summary>
        Hp,
        /// <summary>
        /// Giáp hiện tại
        /// </summary>
        Shield,
        /// <summary>
        /// Thanh dẻo dai
        /// </summary>
        Toughness,
        /// <summary>
        /// Điểm ulti
        /// </summary>
        UltimatePoint,
        /// <summary>
        /// Điểm action hồi dựa theo Speed, đủ 1000 trong round sẽ được đánh<br/>
        /// ap cao nhất được đánh đầu tiên trong round tăng lợi thế, nếu không đủ AP trong round thì mất lượt đánh
        /// </summary>
        ActionPoint,
    }
    public enum FactionAttributeType
    {
        /// <summary>
        /// Mỗi phe có duy nhất một thanh điểm skill dùng chung giống honkaistarail
        /// </summary>
        SkillPoint
    }
}