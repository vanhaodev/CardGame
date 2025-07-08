using System;
using System.Collections.Generic;
using System.Linq;
using Functions.World.Player.Inventory;

namespace Functions.World.Player.Popup.ItemSelector
{
    public class ItemSelectorFilterModel
    {
        /// <summary>
        /// -1 all | 0. equip | 1. resource | 2. card shard
        /// </summary>
        public int ItemTypeNeedIndex = -1;
        public HashSet<uint> ItemTemplateIdWanteds;
        public HashSet<uint> ItemTemplateIdNotWanteds;
        public HashSet<uint> ItemIdWanteds;
        public HashSet<uint> ItemIdNotWanteds;
        /// <summary>
        /// For equipment type
        /// </summary>
        public HashSet<byte> EquipmentTierWanteds;
        public HashSet<ItemRarity> ItemRarityWanteds;
        
        public List<InventoryItemModel> ApplyFilter(List<InventoryItemModel> items)
        {
            if (ItemTypeNeedIndex != -1)
            {
                items = items.Where(i =>
                {
                    if (ItemTypeNeedIndex == 2)
                    {
                        // Shard items – theo template id nằm trong khoảng xác định
                        return i.Item.TemplateId is >= 1001 and <= 1999;
                    }

                    var itemType = i.Item switch
                    {
                        ItemEquipmentModel => ItemType.Equipment,
                        ItemResourceModel => ItemType.Resource,
                        _ => throw new Exception($"Unknown item type: {i.Item?.GetType().Name}")
                    };

                    return (int)itemType == ItemTypeNeedIndex;
                }).ToList();
            }

            if (ItemTemplateIdWanteds != null)
            {
                items = items.Where(i => ItemTemplateIdWanteds.Contains(i.Item.TemplateId)).ToList();
            }

            if (ItemTemplateIdNotWanteds != null)
            {
                items = items.Where(i => !ItemTemplateIdNotWanteds.Contains(i.Item.TemplateId)).ToList();
            }

            if (ItemIdWanteds != null)
            {
                items = items.Where(i => ItemIdWanteds.Contains(i.Item.Id)).ToList();
            }

            if (ItemIdNotWanteds != null)
            {
                items = items.Where(i => !ItemIdNotWanteds.Contains(i.Item.Id)).ToList();
            }

            if (EquipmentTierWanteds != null)
            {
                items = items
                    .Where(i =>
                    {
                        var equip = i.Item as ItemEquipmentModel;
                        return equip != null && EquipmentTierWanteds.Contains(equip.Tier);
                    })
                    .ToList();
            }

            if (ItemRarityWanteds != null)
            {
                items = items.Where(i => ItemRarityWanteds.Contains(i.Item.Rarity)).ToList();
            }

            return items;
        }
    }
}