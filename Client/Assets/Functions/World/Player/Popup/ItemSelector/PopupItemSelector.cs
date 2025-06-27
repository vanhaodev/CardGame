using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Popups;
using UnityEngine;
using World.Player.PopupCharacter;

public class PopupItemSelector : Popup
{
    [SerializeField] private InventoryUI _inventoryUI;

    /// <summary>
    /// 0 all | 1 equipment |2 resources | 3 shards
    /// </summary>
    /// <param name="itemTypeFilterIndex"></param>
    public async UniTask InitItem(ItemActionModel itemActionModel, int itemTypeFilterIndex = -1)
    {
        itemActionModel.OnClose += ()=> Close();
        await _inventoryUI.Init(itemActionModel, itemTypeFilterIndex);
    }
}