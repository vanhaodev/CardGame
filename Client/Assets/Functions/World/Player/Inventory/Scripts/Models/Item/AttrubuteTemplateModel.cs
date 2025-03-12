using System;
using System.Collections.Generic;
using World.Card;

namespace World.Player.Inventory
{
    [Serializable]
    public class AttributeTemplateModel
    {
        public AttributeType Type;
        public int MinValue;
        public int MaxValue;
        public int MinPercentValue;
        public int MaxPercentValue;
    }
    [Serializable]
    public class AttributeRandomLineTemplateModel
    {
        public AttributeTemplateModel Attribute;
        /// <summary>
        /// Using while loop to random until have a Attribute
        /// </summary>
        public ushort Chance;
    }
    [Serializable]
    public class EquipmentItemAttributeTemplateModel
    {
        /// <summary>
        /// Attribute quality classified by item quality
        /// </summary>
        public Dictionary<QualityType, List</*Lines*/AttributeTemplateModel>> AttributeLines;
        /// <summary>
        /// WARNING: When parse from Json, need to order sort by chance with ASC <br/>
        /// Random attribute will make the equipment unique (maybe)
        /// </summary>
        public Dictionary<QualityType, List</*Lines*/AttributeRandomLineTemplateModel>> RandomAttributeLines;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<QualityType,
            Dictionary<byte /*UpgradeLevel*/, List</*Lines*/AttributeRandomLineTemplateModel>>
        > UpgradeAttributeLines;
    }
}