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
        public override async void InitItem(InventoryItemModel item)
        {
            var equipment = item.Item as ItemEquipmentModel;
            EquipmentUpgrade.Item = equipment;
            base.InitItem(item);
            //upgrade
            if (equipment.UpgradeLevel > 0)
            {
                _txName.text += " +" + equipment.UpgradeLevel.ToString();
            }
            //attribute
            _attributeUI.Init(AttributeModel.ToDictionary(equipment.CalculatedAttributes ), AttributeModel.ToDictionary(equipment.CalculatedAttributePercents));
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