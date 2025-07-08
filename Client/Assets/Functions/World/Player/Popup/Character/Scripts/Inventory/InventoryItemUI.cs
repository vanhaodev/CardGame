using System;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using Newtonsoft.Json;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace World.Player.PopupCharacter
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] Image _imgBackground;
        [SerializeField] private Image _imgItemIcon;
        [SerializeField] private Image _imgRarityTag;
        [SerializeField] private TextMeshProUGUI _txItemQuantity;
        [SerializeField] private TextMeshProUGUI _txItemUpgradeLevel;
        [SerializeField] private TextMeshProUGUI _txItemTier;
        [SerializeField] private TextMeshProUGUI _txItemId;
        [SerializeField] private GameObject _objLoadingLock;
        [SerializeField] Sprite[] _spriteBackgrounds;
        [SerializeField] private Sprite[] _spriteRarityTags;
        [SerializeField] private InventoryItemModel _item;
        [SerializeField] private ItemType _itemType;
        protected ItemActionModel _itemActionModel;
        public InventoryItemModel Item => _item;
        public ItemType ItemType => _itemType;

        public void InvokeItemActionOnClose()
        {
            Debug.Log("Invoking item action on close");
            _itemActionModel.OnClose?.Invoke();
        }
        public async UniTask Init(InventoryItemModel inventoryItemModel, ItemActionModel itemActionModel)
        {
            if (inventoryItemModel == null || inventoryItemModel.Quantity <= 0)
            {
                gameObject.SetActive(false);
                Clear();
                return;
            }

            _item = inventoryItemModel;
            _itemActionModel = itemActionModel;

            _objLoadingLock.SetActive(true);
            _itemType = _item.Item switch
            {
                ItemEquipmentModel => ItemType.Equipment,
                ItemResourceModel => ItemType.Resource,
                _ => throw new Exception("Unknown item type " + _itemType)
            };
            _imgBackground.gameObject.SetActive(true);
            _imgItemIcon.gameObject.SetActive(true);
            _txItemQuantity.text = _item.Quantity > 1 ? _item.Quantity.ToString() : string.Empty;
            _imgBackground.sprite = _spriteBackgrounds[(int)_item.Item.Rarity - 1];
            if (ItemType == ItemType.Equipment)
            {
                _imgRarityTag.gameObject.transform.parent.gameObject.SetActive(true);
                _imgRarityTag.sprite = _spriteRarityTags[(int)_item.Item.Rarity - 1];

                var localVarEquipment = (_item.Item as ItemEquipmentModel);
                if (localVarEquipment.UpgradeLevel > 0)
                {
                    _txItemUpgradeLevel.gameObject.transform.parent.gameObject.SetActive(true);
                    _txItemUpgradeLevel.text = "+" + localVarEquipment.UpgradeLevel.ToString();
                }
                else
                {
                    _txItemUpgradeLevel.gameObject.transform.parent.gameObject.SetActive(false);
                }

                _txItemTier.gameObject.transform.parent.gameObject.SetActive(true);
                _txItemTier.text = localVarEquipment.Tier.ToString();
                //test
                _txItemId.text = $"[{_item.Item.Id}]";
            }
            else
            {
                _txItemUpgradeLevel.text = String.Empty;
                _imgRarityTag.sprite = null;
                _imgRarityTag.gameObject.transform.parent.gameObject.SetActive(false);
                _txItemUpgradeLevel.gameObject.transform.parent.gameObject.SetActive(false);
                _txItemTier.gameObject.transform.parent.gameObject.SetActive(false);
            }

            //card shard sẽ lấy icon theo kiểu khác
            bool isCardShard = false;
            if (_itemType == ItemType.Resource)
            {
                var temp = await Global.Instance.Get<GameConfig>().GetItemTemplate(_item.Item.TemplateId);
                if (temp is ItemCardShardTemplateModel cardShard)
                {
                    _imgItemIcon.sprite =
                        await Global.Instance.Get<GameConfig>().GetCardShardIcon(cardShard.CardTemplateId);
                    isCardShard = true;
                }
            }

            if (!isCardShard)
            {
                _imgItemIcon.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(_item.Item.TemplateId);
            }

            gameObject.SetActive(true);
            _objLoadingLock.SetActive(false);
        }

        public void OnTouch()
        {
            if (_objLoadingLock.activeSelf) return;

            //sao ko gắn _itemActionModel? vì mỗi item một action nếu set thì luôn bị ghi đè item cuối cùng
            var itemAction = new ItemActionModel()
            {
                OnChangedData = () =>
                {
                    Init(_item, _itemActionModel);
                    _itemActionModel.OnChangedData?.Invoke(); //có chạy
                },
                OnRespawnItemList = _itemActionModel.OnRespawnItemList,
                OnEquip = _itemActionModel.OnEquip != null ? (item) => _itemActionModel.OnEquip?.Invoke(item) : null,
                OnUnequip = _itemActionModel.OnUnequip != null
                    ? (item) => _itemActionModel.OnUnequip?.Invoke(item)
                    : null
            };

            _itemActionModel.OnClose += () =>
            {
                Debug.Log($"OnClose: InventoryItemUI OnTouch");
                itemAction.OnClose?.Invoke();
            };
            if (_itemType == ItemType.Resource)
            {
                Global.Instance.Get<PopupManager>().ShowItemInfo(_item, itemAction);
            }
            else if (_itemType == ItemType.Equipment)
            {
                Global.Instance.Get<PopupManager>().ShowEquipmentInfo(_item, itemAction);
            }
        }

        public void OnHold()
        {
            if (_objLoadingLock.activeSelf) return;
        }

        public void Clear()
        {
            _item = null;
            _imgBackground.sprite = null;
            _imgBackground.gameObject.SetActive(false);
            _imgItemIcon.sprite = null;
            _imgItemIcon.gameObject.SetActive(false);
            _txItemQuantity.text = String.Empty;

            _imgRarityTag.gameObject.transform.parent.gameObject.SetActive(false);
            _txItemTier.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}