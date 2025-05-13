using System.Collections.Generic;

namespace World.Requirement
{
    /// <summary>
    /// nhóm theo templateid để tăng hiệu suất tìm kiếm
    /// </summary>
    public class InventoryGroupModel
    {
         public ushort ItemTemplateId;

        // Dictionary<itemInvId, List<slot>>
        public Dictionary<int /*itemInvId*/, List<InventoryItemModel>> PartialSlots =
            new Dictionary<int, List<InventoryItemModel>>();

        public Dictionary<int /*itemInvId*/, List<InventoryItemModel>> FullSlots =
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

                int itemInvId = itemSlot.Item.InventoryUniqueId;

                if (stackLimit == 1 || itemSlot.Quantity >= stackLimit)
                {
                    // Thêm vào FullSlots với cấu trúc Dictionary<int, List<InventoryItemModel>>
                    if (!FullSlots.ContainsKey(itemInvId))
                    {
                        FullSlots[itemInvId] = new List<InventoryItemModel>();
                    }

                    FullSlots[itemInvId].Add(itemSlot);
                }
                else
                {
                    // Thêm vào PartialSlots như trước đây
                    if (!PartialSlots.ContainsKey(itemInvId))
                    {
                        PartialSlots[itemInvId] = new List<InventoryItemModel>();
                    }

                    PartialSlots[itemInvId].Add(itemSlot);
                }
            }
        }
    }
}