using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tab;
using World.TheCard;

namespace World.Player.PopupCharacter
{
    public class PopupEquipment : PopupItem
    {
        [SerializeField] private TabSwitcher _tab;
        [SerializeField] private TextMeshProUGUI _txRequiredLevel;
        [SerializeField] private CardAttributeUI _attributeUI;
        [SerializeField] RectTransform _rectTransformContent;
        public static ItemEquipmentModel EquipmentItem;

        private void OnDisable()
        {
            _tab.OnTabSwitched -= ReInit;
        }

        public void SetItem(InventoryItemModel item)
        {
            _item = item;
            EquipmentItem = item.Item as ItemEquipmentModel;
            _tab.Init();
            _tab.OnTabSwitched += ReInit;
        }

        private void ReInit(int _)
        {
            if (_tab.CurrentIndex != 0) return;
            Init(_item, _onChanged);
        }

        public override async void Init(InventoryItemModel item /*null đấy đừng dùng*/, Action onChanged)
        {
            var template = await Global.Instance.Get<GameConfig>().GetItemTemplate(_item.Item.TemplateId);
            base.Init(_item, onChanged);
            //upgrade
            if (EquipmentItem.UpgradeLevel > 0)
            {
                _txName.text += " +" + EquipmentItem.UpgradeLevel.ToString();
            }

            _txRequiredLevel.text = $"Required Level: {(template as ItemEquipmentTemplateModel).RequiredLevel}"; //nếu null thì chắc đang config sai itemtype của item
            //attribute
            _attributeUI.Init(AttributeModel.ToDictionary(EquipmentItem.CalculatedAttributes),
                AttributeModel.ToDictionary(EquipmentItem.CalculatedAttributePercents));
            //
            await _attributeUI.RefreshUI();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransformContent);
        }
    }
}