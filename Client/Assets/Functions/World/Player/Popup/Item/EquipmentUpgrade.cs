using System;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tab;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    public class EquipmentUpgrade : MonoBehaviour
    {
        [SerializeField] private GameObject _objMaxLevel;
        [SerializeField] private GameObject _objNotMaxLevel;
        [SerializeField] private Button _btnUpgrade;
        [SerializeField] private TextMeshProUGUI _txScrapCost;
        [SerializeField] private TextMeshProUGUI _txUpgradeLevel;
        [SerializeField] private TextMeshProUGUI _txSuccessRate;
        [SerializeField] private TextMeshProUGUI _txIncreaseRate;
        private bool _isHasScrap;
        private ushort _successRate;
        private uint _scrapCost;
        public static ItemEquipmentModel Item;
        private ItemEquipmentModel _item => EquipmentUpgrade.Item;

        private void OnEnable()
        {
            _btnUpgrade.onClick.AddListener(OnUpgrade);
            Init();
        }

        private void OnDisable()
        {
            _btnUpgrade.onClick.RemoveListener(OnUpgrade);
        }

        public void Init()
        {
            _btnUpgrade.interactable = false;
            _scrapCost = Global.Instance.Get<GameConfig>().UpgradeItemScrapCost((byte)(_item.UpgradeLevel + 1));
            _successRate = Global.Instance.Get<GameConfig>().UpgradeItemSuccessRate((byte)(_item.UpgradeLevel + 1));
            _objMaxLevel.SetActive(_successRate == 0);
            _objNotMaxLevel.SetActive(_successRate != 0);
            if (_successRate == 0)
            {
                return;
            }

            // Lấy object currency kiểu Scrap trong danh sách
            var charData = Global.Instance.Get<CharacterData>();
            var scrapCurrency = charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap);
            uint scrapAmount = scrapCurrency != null ? (uint)scrapCurrency.Amount : 0;
            _isHasScrap = scrapAmount > _scrapCost;

            var increasePercentCurrent = Global.Instance.Get<GameConfig>().UpgradeItemPercentBonus(_item.UpgradeLevel);
            var increasePercentNext = Global.Instance.Get<GameConfig>().UpgradeItemPercentBonus((byte)(_item.UpgradeLevel+1));
            _txScrapCost.text = $"{(_isHasScrap ? "" : "<color=red>")}" + _scrapCost;
            _txUpgradeLevel.text = $"+{_item.UpgradeLevel} <color=#6BFF00>-> +{_item.UpgradeLevel + 1}</color>";
            _txIncreaseRate.text = $"{increasePercentCurrent:0.##}% -> <color=#6BFF00>{increasePercentNext:0.##}% Increase";
            _txSuccessRate.text = $"{_successRate / 100f:0.##}% Success rate";
            _btnUpgrade.interactable = true;
        }

        private void OnUpgrade()
        {
            if (_isHasScrap == false)
            {
                Global.Instance.Get<PopupManager>().ShowToast("You don't have enough Scrap");
                return;
            }
        }
    }
}