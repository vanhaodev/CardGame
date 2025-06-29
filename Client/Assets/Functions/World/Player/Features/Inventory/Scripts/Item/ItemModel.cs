using System;

namespace Functions.World.Player.Inventory
{
    [System.Serializable]
    public abstract class ItemModel
    {
        public uint Id;
        public uint TemplateId;
        public ItemRarity Rarity;
        /// <summary>
        /// UTC
        /// </summary>
        public DateTime CreatedAt;
        /// <summary>
        /// UTC
        /// </summary>
        public DateTime? ExpiresAt;
    }
}