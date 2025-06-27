using UnityEngine.Events;

namespace Functions.World.Player.Inventory
{
    public class ItemActionModel
    {
        public UnityAction OnChangedData;
        public UnityAction<ItemModel> OnChangedDataWithModel;
        public UnityAction<ItemModel> OnEquip;
        public UnityAction<ItemModel> OnUnequip;
        /// <summary>
        /// có thể dunng để đóng popup nếu làm gì đó thành công
        /// </summary>
        public UnityAction OnClose;
    }
}