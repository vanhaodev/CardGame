using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Functions.World.Player.Popup.ItemSelector;
using GameConfigs;
using Globals;
using Popups;
using UnityEngine;
using UnityEngine.Events;
using Utils.Tab;
using World.Player.Character;
using World.TheCard;

public class PopupCardEquipment : MonoBehaviour, ITabSwitcherWindow
{
    [SerializeField] private InventoryEquipmentSlot[] _slots;
    private CardModel _cardModel;
    private int _currentSlotIndex;
    private UnityAction _onClosePopupAfterUseItem;

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
        _onClosePopupAfterUseItem = () => { };
        for (int i = 0; i < _slots.Length; i++)
        {
            ItemEquipmentModel equipment = null;

            if (_cardModel.Equipments.TryGetValue((byte)i, out var foundEquip))
            {
                equipment = foundEquip.equipment;
            }

            await _slots[i].InitSlot(equipment, equipment != null ? OnUnSelect : null,
                () => _cardModel.UpdateAttribute());
            _slots[i].InitLevelRequirement(_cardModel.Level.GetLevel(i == 0));
        }

        Debug.Log("Check _onClosePopupAfterUseItem is null == " + (_onClosePopupAfterUseItem == null));
    }

    void SelectSlot(InventoryItemSelectSlot slot)
    {
        _currentSlotIndex = Array.IndexOf(_slots, slot);
        Debug.Log(slot.Identity);
        if (slot.IsEmpty)
        {
            var theAc = new ItemActionModel()
            {
                // OnChangedData = () =>
                // {
                //     Debug.Log("Item in equipment has changed");
                //     _cardModel.UpdateAttribute();
                // }, //empty no need changed
                OnEquip = OnSelect
            };
            _onClosePopupAfterUseItem = () =>
            {
                Debug.Log(" void SelectSlot(InventoryItemSelectSlot slot)   slot.IsEmpty");
                theAc.OnClose?.Invoke();
            };
            Global.Instance.Get<PopupManager>().ShowItemSelector(theAc, new ItemSelectorFilterModel()
            {
                ItemTypeFilterIndex = (int)(ItemType.Equipment + 1),
            });
            return;
        }

        slot.ShowItemInfor(ref _onClosePopupAfterUseItem);
    }

    //Thật ra unbselect không hề gắn đúng 
    async void OnSelect(ItemModel item)
    {
        Debug.Log("Select " + item.Id);

        var temp =
            await Global.Instance.Get<GameConfig>().GetItemTemplate(item.TemplateId) as ItemEquipmentTemplateModel;

        if (_cardModel.Level.GetLevel(false) < temp.RequiredLevel)
        {
            Global.Instance.Get<PopupManager>()
                .ShowChoice($"You need to be at least Level {temp.RequiredLevel}.");
            return;
        }

        if (temp.EquipmentType == EquipmentType.Boots)
        {
            foreach (var e in _cardModel.Equipments.Values)
            {
                var template =
                    await Global.Instance.Get<GameConfig>().GetItemTemplate(e.equipment.TemplateId) as
                        ItemEquipmentTemplateModel;
                if (template.EquipmentType == EquipmentType.Boots)
                {
                    Global.Instance.Get<PopupManager>()
                        .ShowChoice($"You already wear boots.");
                    return;
                }
            }
        }
        else
        {
            //có equipment có thể mặc nhiều equipment cùng loại
            int sameEquipCount = _cardModel.Equipments.Values.Count(e => e.equipment.TemplateId == item.TemplateId);
            bool isMaxSame = sameEquipCount >= temp.DuplicateEquipCount;
            if (isMaxSame)
            {
                Global.Instance.Get<PopupManager>()
                    .ShowChoice($"You already wear this type. Max allowed: {temp.DuplicateEquipCount}.");
                return;
            }
        }

        if (!_cardModel.Equipments.ContainsKey((byte)_currentSlotIndex))
        {
            _cardModel.Equipments[(byte)_currentSlotIndex] = new CardEquipmentModel();
        }

        _cardModel.Equipments[(byte)_currentSlotIndex].equipment = item as ItemEquipmentModel;

        Global.Instance.Get<CharacterData>().CharacterModel.Inventory.RemoveItem(item.Id, 1);
        _onClosePopupAfterUseItem.Invoke();
        await UniTask.WhenAll(
            _cardModel.UpdateAttribute(),
            InitSlots()
        );
    }

    async void OnUnSelect(ItemModel item)
    {
        var isEnoughInvSlot = await Global.Instance.Get<CharacterData>().CharacterModel.Inventory
            .IsWeightEnough(item.TemplateId, 1);
        if (!isEnoughInvSlot)
        {
            Global.Instance.Get<PopupManager>()
                .ShowToast("You don't have enough inventory space", PopupToastSoundType.Error);
            return;
        }

        if (_cardModel.Equipments.ContainsKey((byte)_currentSlotIndex))
        {
            _cardModel.Equipments.Remove((byte)_currentSlotIndex);
        }
        else
        {
            throw new Exception("Slot not found");
        }

        Global.Instance.Get<CharacterData>().CharacterModel.Inventory.Items.Add(new InventoryItemModel()
        {
            Item = item,
            Quantity = 1
        });
        _onClosePopupAfterUseItem.Invoke();
        await UniTask.WhenAll(
            _cardModel.UpdateAttribute(),
            InitSlots()
        );
        Debug.Log("OnUnequip " + item.Id);
    }

    public UniTask LateInit()
    {
        return UniTask.CompletedTask;
    }
}