using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Functions.World.Player.Popup.ItemSelector;
using GameConfigs;
using Globals;
using Newtonsoft.Json;
using Popups;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils.Tab;
using World.Player.Character;
using World.Player.PopupCharacter;
using World.TheCard;

public class EquipmentTierUpgrade : MonoBehaviour, ITabSwitcherWindow
{
    [SerializeField] private InventoryEquipmentSlot _mainEquipment;
    [SerializeField] private InventoryEquipmentSlot[] _equipmentNeedSlots;
    private int _currentSlotIndex;
    private Dictionary<byte /*slot index*/, ItemEquipmentModel> _selectedEquipments;
    private UnityAction _onClosePopupAfterUseItem;
    private ItemEquipmentModel _itemInfo;

    private void Awake()
    {
        for (int i = 0; i < _equipmentNeedSlots.Length; i++)
        {
            _equipmentNeedSlots[i].Identity = i.ToString();
            _equipmentNeedSlots[i].ListenOnClick(SelectEquipmentNeedSlot);
        }
    }

    public async UniTask Init(TabSwitcherWindowModel model = null)
    {
        _selectedEquipments = new();
        var equipment = (model as TabSwitcherWindowPopupEquipmentModel).Item.Item as ItemEquipmentModel;
        _itemInfo = equipment;
        _mainEquipment.InitSlot(equipment, null, null);
        await InitEquipmentNeedSlots();
    }

    async UniTask InitEquipmentNeedSlots()
    {
        _onClosePopupAfterUseItem = () => { };
        for (int i = 0; i < _equipmentNeedSlots.Length; i++)
        {
            ItemEquipmentModel equipment = null;

            if (_selectedEquipments.TryGetValue((byte)i, out var result))
            {
                equipment = result;
            }

            await _equipmentNeedSlots[i].InitSlot(equipment, equipment != null ? OnUnSelect : null,
                null);
            _equipmentNeedSlots[i].InitLevelRequirement(0);
        }
    }

    void SelectEquipmentNeedSlot(InventoryItemSelectSlot slot)
    {
        _currentSlotIndex = Array.IndexOf(_equipmentNeedSlots, slot);
        Debug.Log(slot.Identity);
        if (slot.IsEmpty)
        {
            var theAc = new ItemActionModel()
            {
                OnEquip = OnSelectEquipmentNeedSlot
            };
            _onClosePopupAfterUseItem = () =>
            {
                Debug.Log(" void SelectSlot(InventoryItemSelectSlot slot)   slot.IsEmpty");
                theAc.OnClose?.Invoke();
            };
            var itemIdNotWanteds = new HashSet<uint>(
                new[] { _itemInfo.Id }
                    .Concat(
                        _selectedEquipments.Values
                            .Select(e => e.Id)
                    )
            );
            Debug.Log(JsonConvert.SerializeObject(itemIdNotWanteds));
            Global.Instance.Get<PopupManager>().ShowItemSelector(theAc, new ItemSelectorFilterModel()
            {
                ItemTypeFilterIndex = (int)(ItemType.Equipment + 1),
                ItemTemplateIdWanteds = new HashSet<uint>() { _itemInfo.TemplateId },
                ItemIdNotWanteds = itemIdNotWanteds,
                EquipmentTierWanteds = new HashSet<byte>() { _itemInfo.Tier }
            });
            return;
        }

        slot.ShowItemInfor(ref _onClosePopupAfterUseItem);
    }

    async void OnSelectEquipmentNeedSlot(ItemModel item)
    {
        Debug.Log("Select " + item.Id);
        var temp =
            await Global.Instance.Get<GameConfig>().GetItemTemplate(item.TemplateId) as ItemEquipmentTemplateModel;

        if (!_selectedEquipments.ContainsKey((byte)_currentSlotIndex))
        {
            _selectedEquipments[(byte)_currentSlotIndex] = new();
        }

        _selectedEquipments[(byte)_currentSlotIndex] = item as ItemEquipmentModel;
        _onClosePopupAfterUseItem.Invoke();
        await UniTask.WhenAll(
            InitEquipmentNeedSlots()
        );
    }

    async void OnUnSelect(ItemModel item)
    {
        if (_selectedEquipments.ContainsKey((byte)_currentSlotIndex))
        {
            _selectedEquipments.Remove((byte)_currentSlotIndex);
        }
        else
        {
            throw new Exception("Slot not found");
        }

        _onClosePopupAfterUseItem.Invoke();
        await UniTask.WhenAll(
            InitEquipmentNeedSlots()
        );
        Debug.Log("OnUnequip " + item.Id);
    }

    public async UniTask LateInit()
    {
    }
}