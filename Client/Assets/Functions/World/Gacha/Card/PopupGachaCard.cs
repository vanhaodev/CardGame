using System;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using Popups;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tab;
using World.Player.Character;

namespace Functions.World.Gacha
{
    public class PopupGachaCard : PopupGacha
    {
        [BoxGroup("General")] [SerializeField] private Image _imageDNAIconNormal;
        [BoxGroup("General")] [SerializeField] private Image _imageDNAIconStandard;
        [BoxGroup("General")] [SerializeField] private Image _imageDNAIconDeluxe;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txDNANormalAmount;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txDNAStandardAmount;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txDNADeluxeAmount;

        public override async UniTask SetupData()
        {
            await base.SetupData();
            _imageDNAIconNormal.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(1);
            _imageDNAIconStandard.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(2);
            _imageDNAIconDeluxe.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(3);
            InitInventoryDNA();
        }

        private void InitInventoryDNA()
        {
            var charData = Global.Instance.Get<CharacterData>();
            var items = charData.CharacterModel.Inventory.Items;

            _txDNANormalAmount.text = (items.Find(i => i.Item.TemplateId == 1)?.Quantity.ToString()) ?? "0";
            _txDNAStandardAmount.text = (items.Find(i => i.Item.TemplateId == 2)?.Quantity.ToString()) ?? "0";
            _txDNADeluxeAmount.text = (items.Find(i => i.Item.TemplateId == 3)?.Quantity.ToString()) ?? "0";
        }


        protected override async void OnTabSwitched(int index)
        {
            base.OnTabSwitched(index);
            switch (index)
            {
                case 0:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(1);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(1);
                    break;
                }
                case 1:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(2);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(2);
                    break;
                }
                case 2:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(3);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(3);
                    break;
                }
            }

            _txPlayGachaPriceAmountX1.text = "1";
            _txPlayGachaPriceAmountX10.text = "10";
        }
    }
}