using System.Collections.Generic;
using World.TheCard;

namespace World.Requirement
{
    public class ItemAttributeModel
    {
        public AttributeType Type;
        public int Value;
        public int PercentValue;
    }

    public class ItemUpgradeAttributeModel
    {
        public byte UpgradeLevel;
        public List<ItemAttributeModel> Attributes;
    }


    public class SkillLearnBonusAttributeModel
    {
        public AttributeType Type;
        public int Value;
        public int PercentValue;
    }


    public class SkillCallingAttributeModel
    {
        public AttributeType Type;
        public int PercentValue;
    }


    public class SkillDamageAttributeModel
    {
        public AttributeType Type;
        public int PercentValue;
    }


    public class EffectCallingAttributeModel
    {
        public AttributeType Type;
        public int PercentValue;
    }
}