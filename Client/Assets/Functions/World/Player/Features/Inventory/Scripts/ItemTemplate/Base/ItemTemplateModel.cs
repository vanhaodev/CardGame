using System;
using System.Collections.Generic;
using UnityEngine;

namespace Functions.World.Player.Inventory
{
    [Serializable]
    public abstract class ItemTemplateModel
    {
        public uint Id;
        public string ItemName;
        public ItemType ItemType;
        /// <summary>
        /// Base rarity, can dynamic in equipment
        /// </summary>
        public ItemRarity Rarity = ItemRarity.N;
        [TextArea] public string Description;

        /// <summary>
        /// Khối lượng của item (Kilogram)
        /// </summary>
        public uint Weight = 1;

        //==========[Giá bán cho shop]=============//
        public uint SellToShopScrapPrice;
        public uint SellToShopCircuitPrice;

        /// <summary>
        /// Hạn sử dụng tính bằng phút
        /// </summary>
        public int LifeSpanMinute = -1;
    }
}