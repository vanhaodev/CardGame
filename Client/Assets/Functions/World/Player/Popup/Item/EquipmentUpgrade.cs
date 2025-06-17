using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.Tab;
using World.Player.Character;
using Random = UnityEngine.Random;

namespace World.Player.PopupCharacter
{
    public class EquipmentUpgrade : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] private GameObject _objBlockInput;
        [SerializeField] private GameObject _objMaxLevel;
        [SerializeField] private GameObject _objNotMaxLevel;
        [SerializeField] private Button _btnUpgrade;
        [SerializeField] private TextMeshProUGUI _txScrapCost;
        [SerializeField] private TextMeshProUGUI _txUpgradeLevel;
        [SerializeField] private TextMeshProUGUI _txSuccessRate;
        [SerializeField] private TextMeshProUGUI _txIncreaseRate;
        [SerializeField] private ParticleSystem _particleSuccess;
        [SerializeField] private ParticleSystem _particleFail;
        [SerializeField] private GameObject _objProgressLoad;
        [SerializeField] private Image _imgProgressLoadFill;
        private bool _isHasScrap;
        private ushort _successRate;
        private uint _scrapCost;
        private uint _myScrap;
        private Action _onChange;
        private Action _onRegularChange;
        private ItemEquipmentModel EquipmentItem => PopupEquipment.EquipmentItem;

        private void OnEnable()
        {
            _btnUpgrade.onClick.AddListener(OnUpgrade); ;
        }

        private void OnDisable()
        {
            _btnUpgrade.onClick.RemoveListener(OnUpgrade);
            _onChange?.Invoke();
        }

        public async UniTask Init(TabSwitcherWindowModel model = null)
        {
            if (model != null)
            {
                _onChange = model.OnChanged;
                _onRegularChange = model.OnRegularChanged;
            }
            _btnUpgrade.interactable = false;

            RefreshUpgradeData();

            if (_successRate == 0)
            {
                _objMaxLevel.SetActive(true);
                _objNotMaxLevel.SetActive(false);
                return;
            }

            RefreshScrapStatus();
            RefreshUI();
            _btnUpgrade.interactable = true;
        }

        private async void OnUpgrade()
        {
            _btnUpgrade.interactable = false;
            _objBlockInput.SetActive(true);
            RefreshUpgradeData();
            RefreshScrapStatus();

            if (!_isHasScrap)
            {
                _btnUpgrade.interactable = true;
                Global.Instance.Get<PopupManager>().ShowToast("You don't have enough Scrap");
                _objBlockInput.SetActive(false);
                return;
            }

            var charData = Global.Instance.Get<CharacterData>();
            var scrapCurrency = charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap);
            scrapCurrency.Amount -= _scrapCost;
            _imgProgressLoadFill.fillAmount = 0;
            _objProgressLoad.SetActive(true);
            var itemNeedToUp = EquipmentItem;
            bool isSuccess = Random.Range(1, 10001) < _successRate;
            await _imgProgressLoadFill.DOFillAmount(1, 1).WithCancellation(destroyCancellationToken);
            if (isSuccess)
            {
                Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_UpgradeSuccess");
                _particleSuccess.Stop();
                _particleSuccess.Play();
                itemNeedToUp.UpgradeLevel += 1;
                await itemNeedToUp.UpdateAttribute();
                _onRegularChange?.Invoke();
                Init(); // Refresh lại toàn bộ UI sau khi nâng cấp
            }
            else
            {
                Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_UpgradeFail");
                _particleFail.Stop();
                _particleFail.Play();
            }

            _objProgressLoad.SetActive(false);
            _btnUpgrade.interactable = true;
            _objBlockInput.SetActive(false);
        }

        private void RefreshUpgradeData()
        {
            _scrapCost = Global.Instance.Get<GameConfig>().UpgradeItemScrapCost((byte)(EquipmentItem.UpgradeLevel + 1));
            _successRate = Global.Instance.Get<GameConfig>().UpgradeItemSuccessRate((byte)(EquipmentItem.UpgradeLevel + 1));
            _objMaxLevel.SetActive(_successRate == 0);
            _objNotMaxLevel.SetActive(_successRate != 0);
        }

        private void RefreshScrapStatus()
        {
            var charData = Global.Instance.Get<CharacterData>();
            var scrapCurrency = charData.CharacterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap);
            _myScrap = scrapCurrency != null ? (uint)scrapCurrency.Amount : 0;
            _isHasScrap = _myScrap >= _scrapCost;
            charData.InvokeOnCharacterChanged();
        }

        private void RefreshUI()
        {
            var increasePercentCurrent = Global.Instance.Get<GameConfig>().UpgradeItemPercentBonus(EquipmentItem.UpgradeLevel);
            var increasePercentNext = Global.Instance.Get<GameConfig>()
                .UpgradeItemPercentBonus((byte)(EquipmentItem.UpgradeLevel + 1));

            _txScrapCost.text = $"{(_isHasScrap ? "" : "<color=red>")}{_scrapCost}";
            _txUpgradeLevel.text = $"+{EquipmentItem.UpgradeLevel} <color=#6BFF00>-> +{EquipmentItem.UpgradeLevel + 1}</color>";
            _txIncreaseRate.text =
                $"{increasePercentCurrent:0.##}% -> <color=#6BFF00>{increasePercentNext:0.##}% Increase";
            _txSuccessRate.text = $"{_successRate / 100f:0.##}% Success rate";
        }

        public UniTask LateInit()
        {
           return UniTask.CompletedTask;
        }
    }
}