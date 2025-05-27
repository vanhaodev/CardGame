using System;
using Cysharp.Threading.Tasks;
using Globals;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using World.Player.Character;

namespace World.Lobby
{
    public class CharacterInfo : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _txName;
        [SerializeField] TextMeshProUGUI _txCircuitCurrency;
        [SerializeField] TextMeshProUGUI _txScrapCurrency;

        private void OnEnable()
        {
            var cd = Global.Instance.Get<CharacterData>();
            cd.OnCharacterChanged.Subscribe(_ => Setup(cd.CharacterModel)).AddTo(this);
            Setup(cd.CharacterModel);
        }

        public async void Setup(CharacterModel characterModel)
        {
            _txName.text = characterModel.Name;
            _txCircuitCurrency.text = characterModel.Currencies.Find(i => i.Type == CurrencyType.Circuit)?.Amount.ToString() ?? "0";
            _txScrapCurrency.text = characterModel.Currencies.Find(i => i.Type == CurrencyType.Scrap)?.Amount.ToString() ??
                             "0";
            await UniTask.DelayFrame(1);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(_txGold.rectTransform);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(_txSliver.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_txCircuitCurrency.transform.parent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_txScrapCurrency.transform.parent.GetComponent<RectTransform>());
        }
    }
}