using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Functions.World.Player.Inventory;
using Functions.World.Player.Popup.ItemSelector;
using GameConfigs;
using Globals;
using Newtonsoft.Json;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.Tab;
using World.Player.Character;
using World.Player.PopupCharacter;
using World.TheCard;

public class EquipmentTierUpgrade : MonoBehaviour, ITabSwitcherWindow
{
    [SerializeField] private GameObject _objBlockInput;
    [SerializeField] private GameObject _objMaxLevel;
    [SerializeField] private GameObject _objNotMaxLevel;
    [SerializeField] private InventoryEquipmentSlot _mainEquipment;
    [SerializeField] private InventoryEquipmentSlot[] _equipmentNeedSlots;
    [SerializeField] private Button _btnUpgrade;
    private int _currentSlotIndex;
    private Dictionary<byte /*slot index*/, ItemEquipmentModel> _selectedEquipments;
    private UnityAction _onClosePopupAfterUseItem;
    private ItemEquipmentModel _itemInfo;

    [SerializeField] private TextMeshProUGUI _txScrapCost;
    [SerializeField] private Image[] _imgProgressLoadFills;
    [SerializeField] private GameObject _objProgressLoad;
    [SerializeField] private ParticleSystem _particleSuccess;
    private bool _isHasScrap;
    private uint _scrapCost;
    private uint _myScrap;
    private UnityAction _onChange;
    private UnityAction _onRegularChange;
    private UnityAction _onRespawnItemList;

    //auto
    [SerializeField] private Button _btnAutoSelectEquirementEquipment;

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
        if (model != null)
        {
            _onChange = model.OnChanged;
            _onRegularChange = model.OnRegularChanged;
            _onRespawnItemList = (model as TabSwitcherWindowPopupEquipmentModel).ItemAction.OnRespawnItemList; 
            var equipment = (model as TabSwitcherWindowPopupEquipmentModel).Item.Item as ItemEquipmentModel;
            _itemInfo = equipment;
        }

        _selectedEquipments = new();
        _mainEquipment.InitSlot(_itemInfo, 1, null, null);
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

            await _equipmentNeedSlots[i].InitSlot(equipment, 1, equipment != null ? OnUnSelect : null,
                null);
            _equipmentNeedSlots[i].InitLevelRequirement(0);
        }

        _btnUpgrade.interactable = _selectedEquipments.Count == 3;
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

            Global.Instance.Get<PopupManager>().ShowItemSelector(theAc, GetFilter());
            return;
        }

        slot.ShowItemInfor(ref _onClosePopupAfterUseItem);
    }

    public ItemSelectorFilterModel GetFilter()
    {
        var itemIdNotWanteds = new HashSet<uint>(
            new[] { _itemInfo.Id }
                .Concat(
                    _selectedEquipments.Values
                        .Select(e => e.Id)
                )
        );
        return new ItemSelectorFilterModel()
        {
            ItemTypeNeedIndex = (int)ItemType.Equipment,
            ItemTemplateIdWanteds = new HashSet<uint>() { _itemInfo.TemplateId },
            ItemIdNotWanteds = itemIdNotWanteds,
            EquipmentTierWanteds = new HashSet<byte>() { _itemInfo.Tier },
            ItemRarityWanteds = new HashSet<ItemRarity>()
            {
                _itemInfo.Rarity
            }
        };
    }

    async void OnSelectEquipmentNeedSlot(ItemModel item)
    {
        Debug.Log("Select " + item.Id);
        // var temp =
        //     await Global.Instance.Get<GameConfig>().GetItemTemplate(item.TemplateId) as ItemEquipmentTemplateModel;

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

    public async void AutoSelectEquipment()
    {
        _objBlockInput.SetActive(true);
        var inv = Global.Instance.Get<CharacterData>().CharacterModel.Inventory;
        await inv.Arrange();
        var items = inv.Items;
        _selectedEquipments.Clear();
        var filter = GetFilter();
        items = filter.ApplyFilter(items);
        for (int i = 0; i < items.Count; i++)
        {
            if (i == 3) break; //3 slots
            _selectedEquipments[(byte)i] = items[i].Item as ItemEquipmentModel;
        }

        if (_selectedEquipments.Count < 3)
        {
            Global.Instance.Get<PopupManager>().ShowToast("You don't have enough equipment to upgrade tier",
                PopupToastSoundType.Error);
        }

        await UniTask.WhenAll(
            InitEquipmentNeedSlots()
        );
        _objBlockInput.SetActive(false);
    }

//===============================================================\\
    private void RefreshUpgradeData()
    {
        _scrapCost = 5000;
        _objMaxLevel.SetActive(_itemInfo.Tier >= 5);
        _objNotMaxLevel.SetActive(_itemInfo.Tier < 5);
    }

    private void RefreshScrapStatus()
    {
        var charData = Global.Instance.Get<CharacterData>();
        var scrapCurrency = charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap);
        _myScrap = scrapCurrency != null ? (uint)scrapCurrency.Amount : 0;
        _isHasScrap = _myScrap >= _scrapCost;

        _txScrapCost.text = $"{(_isHasScrap ? "" : "<color=red>")}{_scrapCost}";
        charData.InvokeOnCharacterChanged();
    }

    public async void Upgrade()
    {
        _btnUpgrade.interactable = false;
        _objBlockInput.SetActive(true);
        RefreshUpgradeData();
        RefreshScrapStatus();

        if (!_isHasScrap)
        {
            _btnUpgrade.interactable = true;
            Global.Instance.Get<PopupManager>().ShowToast("You don't have enough Scrap", PopupToastSoundType.Error);
            _objBlockInput.SetActive(false);
            return;
        }

        var charData = Global.Instance.Get<CharacterData>();
        var scrapCurrency = charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap);
        scrapCurrency.Amount -= _scrapCost;
        foreach (var e in _selectedEquipments)
        {
            Global.Instance.Get<CharacterData>().CharacterModel.Inventory.RemoveItem(e.Value.Id, 1);
        }

        _imgProgressLoadFills[0].fillAmount = 0;
        _imgProgressLoadFills[1].fillAmount = 0;
        _imgProgressLoadFills[2].fillAmount = 0;
        _objProgressLoad.SetActive(true);
        await UniTask.WhenAll(
            _imgProgressLoadFills[0].DOFillAmount(1, 1).WithCancellation(destroyCancellationToken),
            _imgProgressLoadFills[1].DOFillAmount(1, 1).WithCancellation(destroyCancellationToken),
            _imgProgressLoadFills[2].DOFillAmount(1, 1).WithCancellation(destroyCancellationToken)
        );
        Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_UpgradeSuccess");
        _particleSuccess.Stop();
        _particleSuccess.Play();
        _itemInfo.Tier += 1;
        await _itemInfo.UpdateAttribute();
        _onRegularChange?.Invoke();
        _onRespawnItemList?.Invoke();
        Init();
        _objProgressLoad.SetActive(false);
        _btnUpgrade.interactable = true;
        _objBlockInput.SetActive(false);
    }

    public async UniTask LateInit()
    {
    }

    private void OnDisable()
    {
        _onChange?.Invoke();
    }
}