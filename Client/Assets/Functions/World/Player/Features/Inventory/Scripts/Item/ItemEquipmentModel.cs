using System.Collections.Generic;
using World.TheCard;

namespace Functions.World.Player.Inventory
{
    [System.Serializable]
    public class ItemEquipmentModel : ItemModel
    {
        /// <summary>
        /// tăng chỉ số static vào người đeo
        /// </summary>
        public List<AttributeModel> Attributes;
        /// <summary>
        /// Tăng chỉ số người đeo theo %
        /// </summary>
        public List<AttributeModel> AttributePercents;
    }
}