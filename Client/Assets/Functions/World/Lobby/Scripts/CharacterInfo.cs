using System;
using Globals;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using World.Player.Character;

namespace World.Lobby
{
    public class CharacterInfo : MonoBehaviour
    {
        [SerializeField] Image _imageAvatar;
        [SerializeField] Image _imageAvatarFrame;
        [SerializeField] TextMeshProUGUI _txName;
        [SerializeField] TextMeshProUGUI _txGold;
        [SerializeField] TextMeshProUGUI _txSliver;

        private void OnEnable()
        {
            var cd = Global.Instance.Get<CharacterData>();
            cd.OnCharacterChanged.Subscribe(_ => Setup(cd.CharacterModel)).AddTo(this);
            Setup(cd.CharacterModel);
        }

        public void Setup(CharacterModel characterModel)
        {
            _txName.text = characterModel.Name;
            _txGold.text = characterModel.Currencies.Find(i => i.Type == CurrencyType.Gold)?.Amount.ToString() ?? "0";
            _txSliver.text = characterModel.Currencies.Find(i => i.Type == CurrencyType.Sliver)?.Amount.ToString() ??
                             "0";
        }
    }
}