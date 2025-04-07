using System.Collections.Generic;

namespace World.Card.Skill
{
    public class SkillRemoveStatusTemplateModel: SkillEffectTemplateModel
    {
        public short Rate;
        /// <summary>
        /// xoá trạng thái tốt
        /// </summary>
        public bool IsRemoveBuff;
        /// <summary>
        /// Xoá trạng thái xấu
        /// </summary>
        public bool IsRemoveDebuff;
        public RemoveStatusModel Status;
        public abstract class RemoveStatusModel
        {
        }
        public class RandomRemoveStatusModel : RemoveStatusModel
        {
            public byte QuantityMin;
            public byte QuantityMax;
        }
        public class AllRemoveStatusModel : RemoveStatusModel
        {
            
        }
        public class TypeRemoveStatusModel : RemoveStatusModel
        {
            public List<string> Types = new List<string>(); //frezee, burn...
        }
    }
}