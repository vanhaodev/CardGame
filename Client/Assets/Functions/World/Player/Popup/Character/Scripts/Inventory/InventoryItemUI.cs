using System;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Player.PopupCharacter
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] Image _imgBackground;
        [SerializeField] private Image _imgItemIcon;
        [SerializeField] private TextMeshProUGUI _txItemQuantity;
        [SerializeField] private GameObject _objLoadingLock;
        [SerializeField] Sprite[] _spriteBackgrounds;
        [SerializeField] private InventoryItemModel _item;
        [SerializeField] private ItemType _itemType;
        public InventoryItemModel Item => _item;
        public ItemType ItemType => _itemType;

        public async UniTask Init(InventoryItemModel inventoryItemModel)
        {
            if (inventoryItemModel.Quantity < 1) gameObject.SetActive(false);
            _objLoadingLock.SetActive(true);
            _item = inventoryItemModel;
            _itemType = _item.Item switch
            {
                ItemEquipmentModel => ItemType.Equipment,
                ItemResourceModel => ItemType.Resource,
                _ => throw new Exception("Unknown item type")
            };
            _txItemQuantity.text = _item.Quantity > 1 ? _item.Quantity.ToString() : string.Empty;
            _imgBackground.sprite = _spriteBackgrounds[(int)_item.Item.Rarity];

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

            _objLoadingLock.SetActive(false);
        }

        public void OnTouch()
        {
            if (_objLoadingLock.activeSelf) return;
            if (_itemType == ItemType.Resource)
            {
                Global.Instance.Get<PopupManager>().ShowItemInfo(_item, () => { Init(_item); });
            }
            else if (_itemType == ItemType.Equipment)
            {
                Global.Instance.Get<PopupManager>().ShowEquipmentInfo(_item, () => { Init(_item); });
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
            _imgItemIcon.sprite = null;
            _txItemQuantity.text = String.Empty;
        }
    }
}