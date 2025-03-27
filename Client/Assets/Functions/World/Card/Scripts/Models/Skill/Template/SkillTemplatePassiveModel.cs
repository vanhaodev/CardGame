using System.Collections.Generic;

namespace World.Card
{
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
}