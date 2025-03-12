using System.Collections.Generic;

namespace World.Player.Inventory
{
    public class InventoryGroupModel
    {
         public ushort ItemTemplateId;

        // Dictionary<hash, List<slot>>
        public Dictionary<int /*hash*/, List<InventoryItemModel>> PartialSlots =
            new Dictionary<int, List<InventoryItemModel>>();

        public Dictionary<int /*hash*/, List<InventoryItemModel>> FullSlots =
            new Dictionary<int, List<InventoryItemModel>>();

         public int TotalQuantityCache;
        public int TotalSlotCountCache;

        public void SetItems(List<InventoryItemModel> items)
        {
            PartialSlots.Clear();
            FullSlots.Clear();
            TotalQuantityCache = 0;
            TotalSlotCountCache = 0;

            ushort stackLimit = 1; //set test

            foreach (var itemSlot in items)
            {
                if (itemSlot.Quantity <= 0) continue;

                itemSlot.Item.InitializeInventoryUniqueId();

                TotalQuantityCache += itemSlot.Quantity;
                TotalSlotCountCache++;

                int itemHash = itemSlot.Item.InventoryUniqueId;

                if (stackLimit == 1 || itemSlot.Quantity >= stackLimit)
                {
                    // Thêm vào FullSlots với cấu trúc Dictionary<int, List<InventoryItemModel>>
                    if (!FullSlots.ContainsKey(itemHash))
                    {
                        FullSlots[itemHash] = new List<InventoryItemModel>();
                    }

                    FullSlots[itemHash].Add(itemSlot);
                }
                else
                {
                    // Thêm vào PartialSlots như trước đây
                    if (!PartialSlots.ContainsKey(itemHash))
                    {
                        PartialSlots[itemHash] = new List<InventoryItemModel>();
                    }

                    PartialSlots[itemHash].Add(itemSlot);
                }
            }
        }
    }
}