using System;
using System.Collections.Generic;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using Popups;
using Popups.Commons.Choice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    public class PopupItem : Popup
    {
        [SerializeField] private InventoryItemUI _itemUI;
        [SerializeField] protected TextMeshProUGUI _txName;
        [SerializeField] private TextMeshProUGUI _txDescription;
        [SerializeField] private Button _btnUse;
        private uint _sellScrapPrice;
        private uint _sellCircuitPrice;
        protected InventoryItemModel _item;
        protected Action _onChanged;

        public virtual async void Init(InventoryItemModel item, Action onChanged)
        {
            _onChanged = onChanged;
            _onChanged += () => _itemUI.Init(item);
            var template = await Global.Instance.Get<GameConfig>().GetItemTemplate(item.Item.TemplateId);
            _sellScrapPrice = template.SellToShopScrapPrice;
            _sellCircuitPrice = template.SellToShopCircuitPrice;
            _item = item;
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

            bool isUsable = template is ItemCardShardTemplateModel;
            _btnUse.gameObject.SetActive(isUsable);
        }

        public async void Use()
        {
            _btnUse.interactable = false;
            var template = await Global.Instance.Get<GameConfig>().GetItemTemplate(_item.Item.TemplateId);
            switch (template)
            {
                case ItemCardShardTemplateModel cardShard:
                {
                    var requiredShardCount = Global.Instance.Get<GameConfig>().RequiredShardCount;
                    bool isHaveEnough = _item.Quantity >= requiredShardCount;
                    if (isHaveEnough)
                    {
                        
                    }
                    else
                    {
                        Global.Instance.Get<PopupManager>().ShowChoice($"You need to have at least {requiredShardCount} shards to combine the card.");
                    }
                    break;
                }
            }
            _btnUse.interactable = true;
        }
        public void Sell()
        {
            // Giả lập localization (sau thay bằng hệ thống real sau)
            string fmt_1_price = "Sell {0} for {1}";
            string fmt_2_prices = "Sell {0} for {1} and {2}";
            string fmt_scrap = "{0} scrap";
            string fmt_circuit = "{0} circuit";

            string itemName = _txName.text;
            bool hasScrap = _sellScrapPrice > 0;
            bool hasCircuit = _sellCircuitPrice > 0;

            string message;

            if (hasScrap && hasCircuit)
            {
                string price1 = string.Format(fmt_scrap, _sellScrapPrice);
                string price2 = string.Format(fmt_circuit, _sellCircuitPrice);
                message = string.Format(fmt_2_prices, itemName, price1, price2);
            }
            else // chỉ có 1 loại
            {
                string price = hasScrap
                    ? string.Format(fmt_scrap, _sellScrapPrice)
                    : string.Format(fmt_circuit, _sellCircuitPrice);
                message = string.Format(fmt_1_price, itemName, price);
            }

            Action onCloseChoice = null;
            Global.Instance.Get<PopupManager>().ShowChoice(message, new List<ButtonChoiceModel>()
            {
                new ButtonChoiceModel()
                {
                    Content = "Sell",
                    Action = (input) =>
                    {
                        uint quantity = uint.TryParse(input, out var result) ? result : 1;
                        if (quantity > _item.Quantity) quantity = _item.Quantity;
                        var charData = Global.Instance.Get<CharacterData>();
                        if (hasScrap)
                        {
                            var scrapCurrency =
                                charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap);
                            scrapCurrency.Amount += _sellScrapPrice * quantity;
                        }

                        if (hasCircuit)
                        {
                            var circuitCurrency =
                                charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Circuit);
                            circuitCurrency.Amount += _sellCircuitPrice * quantity;
                        }

                        _item.Quantity -= quantity;
                        charData.InvokeOnCharacterChanged();
                        onCloseChoice?.Invoke(); //khi nhấn bán, sẽ đóng popup choice bán
                        _onChanged?.Invoke();
                        if (_item.Quantity < 1)
                        {
                            //nếu hết quantity đóng luôn info
                            Close();
                        }
                    }
                }
            }, this is not PopupEquipment, "1", close => onCloseChoice = close);
        }
    }
}