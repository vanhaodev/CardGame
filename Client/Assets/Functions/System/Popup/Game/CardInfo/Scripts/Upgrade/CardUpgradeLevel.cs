using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Functions.World.Player.Popup.ItemSelector;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using World.Player.Character;
using World.TheCard;

namespace Popups
{
    public partial class CardUpgradeLevel : CardUpgrade
    {
        [SerializeField] private GameObject _objBlockInput;
        [SerializeField] private InventoryItemSelectSlot _itemExpSlot;
        [SerializeField] private Button _btnUpgrade;
        private ItemModel _selectedItem;
        private uint _selectedItemQuantity;
        private UnityAction _onClosePopupAfterUseItem;

        //====================[Level]=====================\\
        [SerializeField] TextMeshProUGUI _txLevel;
        [SerializeField] private TextMeshProUGUI _txLevelExp;
        [SerializeField] private Image _imgLevelExpFill;

        private void Awake()
        {
            _itemExpSlot.ListenOnClick(SelectItemSlot);
        }

        public override async UniTask SetData(CardModel cardModel, ItemActionModel itemAction)
        {
            await base.SetData(cardModel, itemAction);
            await Init();
        }

        private async UniTask Init()
        {
            _txLevel.text = _cardModel.Level.GetLevel().ToString();
            _txLevelExp.text =
                $"{_cardModel.Level.GetExpCurrent(false).ToString()}" +
                "<color=black>/</color>" +
                $"{_cardModel.Level.GetExpNext(false).ToString()}";
            _imgLevelExpFill.fillAmount = _cardModel.Level.GetProgress(false) / 100;

            _onClosePopupAfterUseItem = () => { };
            if (_selectedItem != null)
            {
                var inv = Global.Instance.Get<CharacterData>().CharacterModel.Inventory;
                await inv.Arrange();
                var selectedItem = inv.GetItemByTemplateId(_selectedItem.TemplateId);
                _selectedItemQuantity = selectedItem.Quantity;

                if (_selectedItemQuantity <= 0) _selectedItem = null;
            }

            await _itemExpSlot.InitSlot(_selectedItem, _selectedItemQuantity, _selectedItem != null ? OnUnSelect : null,
                null);

            _btnUpgrade.interactable = _selectedItem != null;
        }

        void SelectItemSlot(InventoryItemSelectSlot slot)
        {
            Debug.Log(slot.Identity);
            if (slot.IsEmpty)
            {
                var theAc = new ItemActionModel()
                {
                    OnEquip = OnSelectItemSlotResult
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

        async void OnSelectItemSlotResult(ItemModel item)
        {
            Debug.Log("Select " + item.Id);
            _selectedItem = item;
            _onClosePopupAfterUseItem.Invoke();
            await UniTask.WhenAll(
                Init()
            );
        }

        async void OnUnSelect(ItemModel item)
        {
            if (_selectedItem != null)
            {
                _selectedItem = null;
            }
            else
            {
                throw new Exception("Item is null");
            }

            _onClosePopupAfterUseItem.Invoke();
            await UniTask.WhenAll(
                Init()
            );
            Debug.Log("OnUnequip " + item.Id);
        }

        public async void Upgrade()
        {
            _btnUpgrade.interactable = false;
            _objBlockInput.SetActive(true);
        }
    }

//---------------------------------------------------------------------------------------------------------------------\\
    public partial class CardUpgradeLevel : CardUpgrade
    {
        public ItemSelectorFilterModel GetFilter()
        {
            return new ItemSelectorFilterModel()
            {
                ItemTemplateIdWanteds = new HashSet<uint>() { 11, 12, 13, 14 },
            };
        }
    }
}