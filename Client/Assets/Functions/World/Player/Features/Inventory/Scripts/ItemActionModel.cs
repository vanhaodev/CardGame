using UnityEngine.Events;

namespace Functions.World.Player.Inventory
{
    public class ItemActionModel
    {
        public UnityAction OnChangedData;
        public UnityAction<ItemModel> OnChangedDataWithModel;
        public UnityAction<ItemModel> OnEquip;
        public UnityAction<ItemModel> OnUnequip;
    }
}