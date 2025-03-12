using System;
using System.Collections.Generic;

namespace World.Player.Inventory
{
    public class InventoryModel
    {
        public byte MaxBagSlot = 20;

        // Nhóm theo ItemTemplateId
        public Dictionary<ushort, InventoryGroupModel> BagItems = new Dictionary<ushort, InventoryGroupModel>();

        // Cache tổng số slot đã dùng
        public int CurrentBagSlotCount;

        /// <summary>
        /// set cooldown for use item
        /// </summary>
        public DateTime UsedItemAt;

        public void SetUseItemCooldown()
        {
            UsedItemAt = DateTime.Now.AddSeconds(1);
        }

        public bool IsUseItemAvailableTime() => UsedItemAt < DateTime.Now;
    }
}
/*
# Tài liệu hệ thống Inventory

## 1. **InventoryModel**
Lớp `InventoryModel` đại diện cho hệ thống túi đồ của người chơi.

### Thuộc tính:
- `MaxBagSlot`: Số ô chứa tối đa trong túi đồ (mặc định là 20).
- `BagItems`: Danh sách vật phẩm trong túi, được nhóm theo `ItemTemplateId`.
- `CurrentBagSlotCount`: Tổng số ô đã sử dụng trong túi.
- `UsedItemAt`: Thời gian cuối cùng sử dụng vật phẩm.

### Phương thức:
- `SetUseItemCooldown()`: Thiết lập cooldown 1 giây sau khi sử dụng vật phẩm.
- `IsUseItemAvailableTime()`: Kiểm tra xem có thể sử dụng vật phẩm hay chưa.

---
## 2. **InventoryGroupModel**
Lớp `InventoryGroupModel` đại diện cho một nhóm vật phẩm cùng loại trong túi đồ.

### Thuộc tính:
- `ItemTemplateId`: ID mẫu vật phẩm của nhóm này.
- `PartialSlots`: Danh sách vật phẩm chưa đầy một stack, lưu theo `hash`.
- `FullSlots`: Danh sách vật phẩm đã đầy stack, lưu theo `hash`.
- `TotalQuantityCache`: Tổng số lượng vật phẩm trong nhóm này.
- `TotalSlotCountCache`: Tổng số ô chứa vật phẩm trong nhóm này.

### Phương thức:
- `SetItems(List<InventoryItemModel> items)`: Thiết lập danh sách vật phẩm trong nhóm, phân loại vào `PartialSlots` hoặc `FullSlots`.

**Ghi chú:**
- Mỗi vật phẩm có một `InventoryUniqueId` riêng.
- Nếu số lượng đạt `stackLimit`, vật phẩm sẽ được đưa vào `FullSlots`.
- Nếu số lượng chưa đủ `stackLimit`, vật phẩm sẽ được đưa vào `PartialSlots`.

---
## 3. **InventoryItemModel**
Lớp `InventoryItemModel` đại diện cho một vật phẩm đơn lẻ trong túi đồ.

### Thuộc tính:
- `Item`: Đối tượng vật phẩm (thuộc lớp `ItemModel`).
- `Quantity`: Số lượng vật phẩm trong ô chứa.

---
## 4. **ItemModel**
Lớp `ItemModel` đại diện cho thông tin chi tiết của một vật phẩm.

### Thuộc tính:
- `Id`: ID duy nhất của vật phẩm.
- `ItemTemplateId`: ID mẫu vật phẩm.
- `IsLocked`: Trạng thái khóa vật phẩm.
- `Quality`: Chất lượng của vật phẩm (`QualityType`).
- `ExpiresAt`: Thời điểm vật phẩm hết hạn (nếu có).
- `CreatedAt`: Thời gian vật phẩm được tạo.
- `UpdatedAt`: Thời gian vật phẩm được cập nhật lần cuối.
- `InventoryUniqueId`: Mã định danh duy nhất của vật phẩm trong túi đồ.

### Phương thức:
- `InitializeInventoryUniqueId()`: Khởi tạo `InventoryUniqueId` dựa trên `ItemTemplateId`, `IsLocked`, `Quality` và `ExpiresAt`.
- `GetInventoryUniqueId()`: Lấy giá trị `InventoryUniqueId`.
- `IsSame(ItemModel other)`: Kiểm tra xem hai vật phẩm có giống nhau không dựa trên `InventoryUniqueId`.
- `IsExpired()`: Kiểm tra xem vật phẩm có hết hạn không.
- `GetExpireInfor()`: Lấy thông tin về hạn sử dụng của vật phẩm (hiển thị thời gian hết hạn hoặc thông báo nếu đã hết hạn).

**Ghi chú:**
- `InventoryUniqueId` giúp hệ thống inventory phân biệt các vật phẩm ngay cả khi chúng có cùng `ItemTemplateId`. Nó dựa trên các yếu tố như `ItemTemplateId`, `IsLocked`, `Quality`, và `ExpiresAt`.
- Nhờ `InventoryUniqueId`, hệ thống có thể nhóm các vật phẩm có cùng thuộc tính vào `PartialSlots` hoặc `FullSlots`, tránh trùng lặp hoặc sai lệch dữ liệu.

---
## 5. **Cơ chế tổ chức vật phẩm trong túi đồ**
- Khi thêm vật phẩm, hệ thống sẽ kiểm tra nếu `stackLimit` là 1 hoặc vật phẩm đã đầy stack thì thêm vào `FullSlots`.
- Nếu chưa đầy stack, vật phẩm được lưu vào `PartialSlots`.
- `TotalQuantityCache` và `TotalSlotCountCache` giúp tối ưu hóa truy vấn số lượng vật phẩm mà không cần duyệt lại danh sách.
*/