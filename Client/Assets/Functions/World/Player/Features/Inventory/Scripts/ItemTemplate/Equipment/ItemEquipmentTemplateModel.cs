using System.Collections.Generic;
using World.TheCard;

namespace Functions.World.Player.Inventory
{
    public enum EquipmentType
    {
        Weapone, //Vũ khí
        Head, // Mũ
        Chest, // Giáp
        Legs, // Quần / giáp chân
        Feet, // Giày
        Ring, // Nhẫn
        Amulet // Dây chuyền
    }

    [System.Serializable]
    
    public class ItemEquipmentTemplateModel : ItemTemplateModel
    {
        /// <summary>
        /// Có được đeo trùng không, có vài item  cho phép đeo dồn, tuy nhiên hiệu ứng đặc biệt của trang bị sẽ không cộng dồn và chỉ lấy của trang bị có hiệu ứng tốt nhất.
        /// </summary>
        public bool IsDuplicateEquip;
        public EquipmentType EquipmentType;
        /// <summary>
        /// tăng chỉ số static vào người đeo
        /// </summary>
        public List<AttributeModel> Attributes;
        /// <summary>
        /// Tăng chỉ số người đeo theo %
        /// </summary>
        public List<AttributeModel> AttributePercents;

        public ItemEquipmentTemplateModel()
        {
            ItemType = ItemType.Equipment;
        }
    }
}