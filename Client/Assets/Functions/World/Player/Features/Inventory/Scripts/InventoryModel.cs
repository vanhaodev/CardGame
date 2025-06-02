using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using UnityEngine;

namespace Functions.World.Player.Inventory
{
    [Serializable]
    public class InventoryModel
    {
        /// <summary>
        /// tính theo Kilogram
        /// </summary>
        public uint MaxWeight;
        public List<InventoryItemModel> Items;
        private int _lastArrangeItemCount;
        public InventoryModel()
        {
            MaxWeight = 1000;
            Items = new List<InventoryItemModel>();
        }

        public async UniTask Arrange()
        {
            //if(Items.Count == _lastArrangeItemCount) return; 
            var grouped = new Dictionary<(uint TemplateId, ItemRarity Rarity), InventoryItemModel>();
            var newItems = new List<InventoryItemModel>();

            foreach (var itemEntry in Items)
            {
                var item = itemEntry.Item;
                var template = await Global.Instance.Get<GameConfig>().GetItemTemplate(item.TemplateId);
                if (template == null)
                    throw new Exception($"Missing template for TemplateId: {item.TemplateId}");

                var itemType = template.ItemType;

                if (itemType == ItemType.Equipment)
                {
                    newItems.Add(itemEntry); // Không gộp equipment
                    continue;
                }

                var key = (item.TemplateId, item.Rarity);
                if (grouped.TryGetValue(key, out var existingGroup))
                {
                    existingGroup.Quantity += itemEntry.Quantity;
                }
                else
                {
                    grouped[key] = new InventoryItemModel
                    {
                        Item = item,
                        Quantity = itemEntry.Quantity
                    };
                }
            }

            newItems.AddRange(grouped.Values);
            newItems.RemoveAll(i => i.Quantity < 1);
            Items = newItems;
            _lastArrangeItemCount = newItems.Count;
        }

        public async UniTask<uint> GetCurrentWeight()
        {
            if (Items == null) return 0;
            var tasks = Items.Select(async item =>
            {
                var itemTemplate = await Global.Instance.Get<GameConfig>().GetItemTemplate(item.Item.TemplateId);
                if (itemTemplate == null)
                    throw new Exception($"ItemTemplate is null for TemplateId: {item.Item.TemplateId}");

                var itemType = itemTemplate.ItemType;
                return new
                {
                    TemplateId = item.Item.TemplateId,
                    Weight = itemTemplate.Weight,
                    Quantity = item.Quantity
                };
            }).ToList();

            var results = await UniTask.WhenAll(tasks);

            uint totalWeight = 0;
            foreach (var result in results)
            {
                totalWeight += result.Weight * result.Quantity;
            }

            return totalWeight;
        }
    }

    [Serializable]
    public class InventoryItemModel
    {
        [SerializeReference] public ItemModel Item;
        public uint Quantity;
    }
}