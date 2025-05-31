using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using UnityEngine;
using UnityEngine.UI;
using World.TheCard;

namespace World.Player.PopupCharacter
{
    public class PopupEquipment : PopupItem
    {
        [SerializeField] private CardAttributeUI _attributeUI;
        [SerializeField] RectTransform _rectTransformContent;
        public override async void InitItem(InventoryItemModel item)
        {
            base.InitItem(item);
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