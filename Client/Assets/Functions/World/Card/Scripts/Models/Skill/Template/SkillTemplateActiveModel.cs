using System.Collections.Generic;

namespace World.Card
{
    public class SkillTemplateActiveModel
    {
        public TargetModel Target;
        public CostModel Cost;
        public AttackModel Attack;
        public BuffModel Buff;
        public DebuffModel Debuff;
        public HealModel Heal;

        public class CooldownModel
        {
            /// <summary>
            /// Khi thực hiện sẽ cần chờ số round
            /// </summary>
            public byte CooldownByRound;
        }

        public class CostModel
        {
            public int HpCost;
            public int MpCost;

            /// <summary>
            /// Ultimate point
            /// </summary>
            public int UpCost;
        }

        public class TargetModel
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
        }

        public class AttackModel
        {
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

        public class BuffModel
        {
            /// <summary>
            /// Thay đổi attribute của mục tiêu
            /// </summary>
            public List<AttributeModel> Attributes;
        }

        public class DebuffModel
        {
            /// <summary>
            /// Thay đổi attribute của mục tiêu
            /// </summary>
            public List<AttributeModel> Attributes;
        }

        public class HealModel
        {
            public int Hp;
            public int Mp;
            public int Up;

            /// <summary>
            /// Hồi dựa vào hp max của mục tiêu
            /// </summary>
            public int HpPercent;

            public int MpPercent;
            public int UpPercent;
        }
    }
}