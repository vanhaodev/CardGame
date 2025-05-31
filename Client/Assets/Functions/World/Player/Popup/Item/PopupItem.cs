using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace World.Player.PopupCharacter
{
    public class PopupItem : Popup
    {
        [SerializeField] private InventoryItemUI _itemUI;
        [SerializeField] private TextMeshProUGUI _txName;
        [SerializeField] private TextMeshProUGUI _txDescription;

        public virtual async void InitItem(InventoryItemModel item)
        {
            var template = await Global.Instance.Get<GameConfig>().GetItemTemplate(item.Item.TemplateId);
             _itemUI.Init(item);
             var starColor = Global.Instance.Get<GameConfig>().GetRarityColor((byte)(item.Item.Rarity + 1));
             _txName.text = template.ItemName;
             _txName.enableVertexGradient = true;
             ColorUtility.TryParseHtmlString(starColor.gradient, out var topColor);
             ColorUtility.TryParseHtmlString(starColor.gradient2, out var bottomColor);
             _txName.colorGradient = new VertexGradient(
                 topColor, topColor, // Top Left, Top Right
                 bottomColor, bottomColor // Bottom Left, Bottom Right
             );
             _txDescription.text = template.Description;
        }
    }
}