using System;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Globals;
using Popups;
using UnityEngine;
using Utils.Tab;
using World.TheCard;

public class PopupCardEquipment : MonoBehaviour, ITabSwitcherWindow
{
    [SerializeField] private InventoryEquipmentSlot[] _slots;
    private CardModel _cardModel;
    private string _currentSlot;
    private void Awake()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].Identity = i.ToString();
            _slots[i].ListenOnClick(SelectSlot);
        }
    }

    public async UniTask Init(TabSwitcherWindowModel model = null)
    {
        var tabSwitcherWindowModel = model as PopupCardTabSwitcherWindowModel;
        _cardModel = tabSwitcherWindowModel.PopupCardModel.CardModel;
        await InitSlots();
    }

     async UniTask InitSlots()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            ItemEquipmentModel equipment = null;

            if (_cardModel.Equipments.TryGetValue((byte)i, out var foundEquip))
            {
                equipment = foundEquip;
            }

            await _slots[i].InitSlot(equipment, equipment != null ? OnUnSelect : null);
            _slots[i].InitLevelRequirement(_cardModel.Level.GetLevel(i == 0));
        }
    }
    void SelectSlot(InventoryItemSelectSlot slot)
    {
        _currentSlot = slot.Identity;
        Debug.Log(slot.Identity);
        if (slot.IsEmpty)
        {
            Global.Instance.Get<PopupManager>().ShowItemSelector(new ItemActionModel()
            {
                OnEquip = OnSelect
            }, 1);
            return;
        }

        slot.ShowItemInfor();
    }

    void OnSelect(ItemModel item)
    {
        Debug.Log("Select " + item.Id);
        InitSlots();
    }
    void OnUnSelect(ItemModel item)
    {
        Debug.Log("OnUnequip " + item.Id);
        InitSlots();
    }
    public UniTask LateInit()
    {
        return UniTask.CompletedTask;
    }
}