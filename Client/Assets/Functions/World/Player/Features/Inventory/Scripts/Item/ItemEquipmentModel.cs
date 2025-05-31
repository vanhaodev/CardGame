using System.Collections.Generic;
using World.TheCard;

namespace Functions.World.Player.Inventory
{
    [System.Serializable]
    public class ItemEquipmentModel : ItemModel
    {
        /// <summary>
        /// Cường hoá + <br/>
        /// Mỗi cấp cộng 0.25% điểm att đang có
        /// </summary>
        public byte UpgradeLevel;
        /// <summary>
        /// tăng chỉ số static vào người đeo
        /// </summary>
        public List<AttributeModel> Attributes;
        public List<AttributeModel> CalculatedAttributes;
        /// <summary>
        /// Tăng chỉ số người đeo theo %
        /// </summary>
        public List<AttributeModel> AttributePercents;
        public List<AttributeModel> CalculatedAttributePercents;
    }
}