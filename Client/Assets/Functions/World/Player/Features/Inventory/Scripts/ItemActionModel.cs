using UnityEngine.Events;

namespace Functions.World.Player.Inventory
{
    public class ItemActionModel
    {
        public UnityAction OnChanged;
        public UnityAction<ItemModel> OnChangedItem;
        public UnityAction<ItemModel> OnEquip;
        public UnityAction<ItemModel> OnUnEquip;
    }
}