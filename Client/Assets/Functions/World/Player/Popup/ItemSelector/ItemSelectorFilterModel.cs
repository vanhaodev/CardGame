using System.Collections.Generic;

namespace Functions.World.Player.Popup.ItemSelector
{
    public class ItemSelectorFilterModel
    {
        public int ItemTypeFilterIndex = -1;
        public HashSet<uint> ItemTemplateIdWanteds;
        public HashSet<uint> ItemTemplateIdNotWanteds;
        public HashSet<uint> ItemIdWanteds;
        public HashSet<uint> ItemIdNotWanteds;
        /// <summary>
        /// For equipment type
        /// </summary>
        public HashSet<byte> EquipmentTierWanteds;
    }
}