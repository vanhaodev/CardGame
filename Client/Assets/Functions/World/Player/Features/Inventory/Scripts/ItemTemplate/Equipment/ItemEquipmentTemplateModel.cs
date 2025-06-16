using System.Collections.Generic;
using UnityEngine;
using World.TheCard;

namespace Functions.World.Player.Inventory
{
    [System.Serializable]
    
    public class ItemEquipmentTemplateModel : ItemTemplateModel
    {
        /// <summary>
        /// Card level need
        /// </summary>
        public byte RequiredLevel;
        /// <summary>
        /// Có được đeo trùng không, có vài item  cho phép đeo dồn, tuy nhiên hiệu ứng đặc biệt của trang bị sẽ không cộng dồn và chỉ lấy của trang bị có hiệu ứng tốt nhất.
        /// </summary>
        public bool IsDuplicateEquip;
        public EquipmentType EquipmentType;
        public byte Tier = 1;
        /// <summary>
        /// tăng chỉ số static vào người đeo
        /// </summary>
        [Header("Tăng thẳng vào chỉ số - Ví dụ 500 crit sẽ tăng thẳng 5% crit")]
        public List<AttributeModel> Attributes;
        /// <summary>
        /// Tăng chỉ số người đeo theo %
        /// </summary>
        [Header("Tăng % chỉ số đang có - Ví dụ 500 crit sẽ tăng 5% của crit đang có")]
        public List<AttributeModel> AttributePercents;

        public ItemEquipmentTemplateModel()
        {
            Tier = 1;
            ItemType = ItemType.Equipment;
            Weight = 25;
        }
    }
}