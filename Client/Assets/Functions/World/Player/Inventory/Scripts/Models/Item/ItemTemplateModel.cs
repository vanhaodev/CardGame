using System;
using UnityEngine;

namespace World.Requirement
{
    [Serializable]
    public class ItemTemplateModel
    {
        public ushort Id;
        public ItemType ItemType;
        public QualityType QualityType;
        public string Name;
        public uint? LifeSpanMinute;
        public RewardModel Sell;
        public string Description;
    }

    [Serializable]
    public class UseableItemTemplateModel : ItemTemplateModel
    {
        public EffectiveModel Effective;
    }

    [Serializable]
    public class EquipmentItemTemplateModel : ItemTemplateModel
    {
        public RequirementModel Requirement;
        public ushort MaxDurability;
        public EquipmentItemAttributeTemplateModel AttributeTemplate;
    }
}