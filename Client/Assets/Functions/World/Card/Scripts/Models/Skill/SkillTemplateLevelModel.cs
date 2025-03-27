using System.Collections.Generic;

namespace World.Card
{
    public class SkillTemplateLevelModel
    {
        public SkillTemplateRequirementModel Requirement;
        public SkillTemplateActiveModel Active;
        public SkillTemplatePassiveModel Passive;
    }

    public class SkillTemplateActiveModel
    {
        /// <summary>
        /// True sẽ được dùng lên chính mình <br/>
        /// Xác định liệu bản thân có được hưởng hiệu ứng hay không khi sử dụng lên đội. <br/>
        /// Điều này phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public bool Self;

        /// <summary>
        /// Số lượng đồng đội (bao gồm cả bản thân nếu áp dụng) sẽ nhận hiệu ứng. <br/>
        /// Số lượng thực tế có thể phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public byte TeamTargetCount;

        /// <summary>
        /// Số lượng kẻ địch sẽ nhận hiệu ứng chỉ khi kích hoạt lên phe địch (vì trường hợp kỹ năng có 2 khả năng hồi cho cả ta và hại cho địch). <br/>
        /// Số lượng thực tế có thể phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public byte EnemyTargetCount;

        /// <summary>
        /// Khi thực hiện sẽ cần chờ số round
        /// </summary>
        public byte CooldownByRound;

        /// <summary>
        /// Skill bonus sát thương hp
        /// </summary>
        public int HpDamageStatic;

        /// <summary>
        /// Skill bonus sát thương mp
        /// </summary>
        public int MpDamageStatic;

        /// <summary>
        /// Sát thương bằng tấn công của nhân vật * % CalingPercent
        /// </summary>
        public int HpDamagePercent;

        /// <summary>
        /// Sát thương bằng tấn công của nhân vật * % CalingPercent
        /// </summary>
        public int MpDamagePercent;

        /// <summary>
        /// Số lần đánh vào một mục tiêu, sát thương cuối = sát thương * AttackCount
        /// </summary>
        public byte AttackCount;
    }

    public class SkillTemplatePassiveModel
    {
        /// <summary>
        /// Tăng điểm attribute theo value
        /// </summary>
        public List<AttributeModel> PassiveValueAttributes;

        /// <summary>
        /// Tăng điểm base attribute theo %
        /// </summary>
        public List<AttributeModel> PassivePercentAttributes;
    }

    public class SkillTemplateCounterModel
    {
        /// <summary>
        /// Điều kiện để kích hoạt
        /// </summary>
        public SkillTemplateRequirementModel Requirement;
        /// <summary>
        /// Số lượng kẻ địch sẽ nhận hiệu ứng chỉ khi kích hoạt lên phe địch (vì trường hợp kỹ năng có 2 khả năng hồi cho cả ta và hại cho địch). <br/>
        /// Số lượng thực tế có thể phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public byte EnemyTargetCount;

        /// <summary>
        /// Khi thực hiện sẽ cần chờ số round
        /// </summary>
        public byte CooldownByRound;

        /// <summary>
        /// Skill bonus sát thương hp
        /// </summary>
        public int HpDamageStatic;

        /// <summary>
        /// Skill bonus sát thương mp
        /// </summary>
        public int MpDamageStatic;

        /// <summary>
        /// Sát thương bằng tấn công của nhân vật * % CalingPercent
        /// </summary>
        public int HpDamagePercent;

        /// <summary>
        /// Sát thương bằng tấn công của nhân vật * % CalingPercent
        /// </summary>
        public int MpDamagePercent;
    }
}