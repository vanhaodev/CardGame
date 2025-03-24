using System.Collections.Generic;

namespace World.Card
{
    public class SkillTemplateLevelModel
    {
        //==================================[Active]====================================\\
        /// <summary>
        /// True sẽ được dùng lên chính mình <br/>
        /// Xác định liệu bản thân có được hưởng hiệu ứng hay không khi sử dụng lên đội. <br/>
        /// Điều này phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public bool? Self;
        /// <summary>
        /// Số lượng đồng đội (bao gồm cả bản thân nếu áp dụng) sẽ nhận hiệu ứng. <br/>
        /// Số lượng thực tế có thể phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public byte? TeamTargetCount;
        /// <summary>
        /// Số lượng kẻ địch sẽ nhận hiệu ứng chỉ khi kích hoạt lên phe địch (vì trường hợp kỹ năng có 2 khả năng hồi cho cả ta và hại cho địch). <br/>
        /// Số lượng thực tế có thể phụ thuộc vào cách tổ chức phạm vi ảnh hưởng (AOE). <br/>
        /// </summary>
        public byte? EnemyTargetCount;
        
        //==================================[Passive]====================================\\
        /// <summary>
        /// Tăng điểm base attribute theo %
        /// </summary>
        public List<AttributeModel> PassivePercentAttributes;
        /// <summary>
        /// Tăng điểm attribute theo value
        /// </summary>
        public List<AttributeModel> PassiveValueAttributes;
    }
}