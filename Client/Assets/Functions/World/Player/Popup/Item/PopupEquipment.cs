using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tab;
using World.TheCard;

namespace World.Player.PopupCharacter
{
    public class PopupEquipment : PopupItem
    {
        [SerializeField] private TabSwitcher _tab;
        [SerializeField] private CardAttributeUI _attributeUI;
        [SerializeField] RectTransform _rectTransformContent;
        public static ItemEquipmentModel EquipmentItem;
        private InventoryItemModel _item;
        private void OnEnable()
        {
            InitItem(_item);
        }

        public void SetItem(InventoryItemModel item)
        {
            _item = item;
            EquipmentItem = item.Item as ItemEquipmentModel;
        }
        public override async void InitItem(InventoryItemModel item)
        {
            base.InitItem(_item);
            //upgrade
            if (EquipmentItem.UpgradeLevel > 0)
            {
                _txName.text += " +" + EquipmentItem.UpgradeLevel.ToString();
            }
            //attribute
            _attributeUI.Init(AttributeModel.ToDictionary(EquipmentItem.CalculatedAttributes ), AttributeModel.ToDictionary(EquipmentItem.CalculatedAttributePercents));
            //
            _tab.Init();
            await _attributeUI.RefreshUI();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransformContent);
        }

        // public override async UniTask Show(float fadeDuration = 0.3f)
        // {
        //     await base.Show(fadeDuration);
        //     await _attributeUI.RefreshUI();
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransformContent);
        // }
    }
}