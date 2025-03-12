using System;
using System.Collections.Generic;
using Utils;

namespace World.Player.Inventory
{
    public class ItemModel
    {
        public uint Id;
        public ushort ItemTemplateId;
        public bool IsLocked;
        public QualityType Quality;
        public DateTime? ExpiresAt;
        public DateTime CreatedAt;
        public DateTime UpdatedAt;

        /// <summary>
        /// Search index in inventory
        /// </summary>
        public int InventoryUniqueId;

        public virtual void InitializeInventoryUniqueId()
        {
            InventoryUniqueId = HashCode.Combine(ItemTemplateId, IsLocked, Quality, ExpiresAt);
        }

        public int GetInventoryUniqueId()
        {
            return InventoryUniqueId;
        }

        public virtual bool IsSame(ItemModel other)
        {
            return other != null && InventoryUniqueId == other.InventoryUniqueId;
        }

        public bool IsExpired()
        {
            return ExpiresAt.HasValue && DateTime.Now > ExpiresAt.Value;
        }

        public string GetExpireInfor()
        {
            if (ExpiresAt == null) return string.Empty;
            string formattedDate = TimeUtils.GetDateTimeDayTime(ExpiresAt.Value);
            return ExpiresAt < DateTime.Now ? $"Đã hết hạn ngày {formattedDate}" : $"Hết hạn vào ngày {formattedDate}";
        }
    }


    public class UseableItemModel : ItemModel
    {
        public byte UseCountRemaining;

        public override void InitializeInventoryUniqueId()
        {
            InventoryUniqueId = (int)Id;
        }
    }


    public class EquipmentItemModel : ItemModel
    {
        public ushort Durability;
        public List<ItemAttributeModel> Attributes;
        public byte UpgradeLevel;
        public List<ItemUpgradeAttributeModel> UpgradeAttributes;

        public override void InitializeInventoryUniqueId()
        {
            InventoryUniqueId = (int)Id;
        }

        public bool IsBroken()
        {
            return Durability <= 0;
        }

        public void MinusDurability(ushort value)
        {
            Durability = (ushort)Math.Max(0, Durability - value);
        }

        // public void Repair(TemplateModel template)
        // {
        //     var itemTempDura = (template.ItemTemplates[ItemTemplateId] as EquipmentItemTemplateModel).MaxDurability;
        //     Durability = itemTempDura;
        // }
    }
}