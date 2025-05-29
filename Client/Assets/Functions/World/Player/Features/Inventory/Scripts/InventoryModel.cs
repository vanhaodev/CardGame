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

        public InventoryModel()
        {
            MaxWeight = 1000;
            Items = new List<InventoryItemModel>();
        }

        public async UniTask<uint> GetCurrentWeight()
        {
            if (Items == null) return 0;

            var tasks = Items.Select(async item =>
            {
                var itemTemplate = await Global.Instance.Get<GameConfig>().GetItemTemplate(item.Item.TemplateId);
                if (itemTemplate == null)
                    throw new Exception($"ItemTemplate is null for TemplateId: {item.Item.TemplateId}");

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