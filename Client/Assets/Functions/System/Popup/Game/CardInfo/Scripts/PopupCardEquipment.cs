using System;
using Cysharp.Threading.Tasks;
using Popups;
using UnityEngine;
using Utils.Tab;
using World.TheCard;

public class PopupCardEquipment : MonoBehaviour, ITabSwitcherWindow
{
    [SerializeField] private InventoryItemSelectSlot[] _slots;
    private CardModel _cardModel;

    private void Awake()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].Identity = i.ToString();
            _slots[i].ListenOnSelect(OnSelectSlot);
        }
    }

    public async UniTask Init(TabSwitcherWindowModel model = null)
    {
        var tabSwitcherWindowModel = model as PopupCardTabSwitcherWindowModel;
        _cardModel = tabSwitcherWindowModel.PopupCardModel.CardModel;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _cardModel.Equipments.Count)
            {
                await _slots[i].InitSlot(_cardModel.Equipments[i]);
            }
            else
            {
                await _slots[i].InitSlot(null);
            }
        }
    }

    void OnSelectSlot(InventoryItemSelectSlot slot)
    {
        Debug.Log(slot.Identity);
        if (slot.IsEmpty)
        {
            return;
        }

        slot.ShowItemInfor();
    }

    public UniTask LateInit()
    {
        return UniTask.CompletedTask;
    }
}