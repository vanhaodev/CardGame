using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Tab;
using World.Player.PopupCharacter;

public class EquipmentTierUpgrade : MonoBehaviour, ITabSwitcherWindow
{
    [SerializeField] private InventoryEquipmentSlot _mainEquipment;
    [SerializeField] private InventoryEquipmentSlot[] _equipmentNeeds;

    public async UniTask Init(TabSwitcherWindowModel model = null)
    {
        var equipment = (model as TabSwitcherWindowPopupEquipmentModel).Item.Item as ItemEquipmentModel;
        _mainEquipment.InitSlot(equipment, null, null);
    }

    public async UniTask LateInit()
    {
    }
}