using System;
using Cysharp.Threading.Tasks;
using Functions.World.Player;
using GameConfigs;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Player.Character;
using World.TheCard;

namespace World.Player.PopupCharacter
{
    public class LineupCard : MonoBehaviour
    {
        [SerializeField] private byte _slotIndex;
        [SerializeField] private Card _card;
        [SerializeField] private Button _btnEquipCard;
        [SerializeField] private GameObject _objEmpty;
        [SerializeField] TextMeshProUGUI _txSlotIndex;
        [SerializeField] TextMeshProUGUI _txCardName;

        public async UniTask Setup(byte slotIndex, int lineupIndex)
        {
            var found = false;
            var charData = Global.Instance.Get<CharacterData>();
            _slotIndex = slotIndex;
            _txSlotIndex.text = slotIndex.ToString();
            var cardLineups = charData.CharacterModel.CardLineups;
            if (lineupIndex - 1 >= 0 &&
                lineupIndex - 1 < cardLineups.Count &&
                cardLineups[lineupIndex - 1] is { Cards: not null } lineup &&
                lineup.Cards.TryGetValue(_slotIndex, out var cardId))
            {
                var cardData = charData.CharacterModel.CardCollection.GetCard(cardId);
                if (cardData == null)
                {
                    found = false;
                }
                else
                {
                    found = true;
                    var cardTemp = await Global.Instance.Get<GameConfig>().GetCardTemplate(cardData.TemplateId);
                    _objEmpty.SetActive(false);
                    _card.CardModel = cardData;
                    _card.gameObject.SetActive(true);
                    _txCardName.text = cardTemp.Name;
                }
            }

            if (found == false)
            {
                _objEmpty.SetActive(true);
                _card.gameObject.SetActive(false);
                _txCardName.text = "";
            }
        }
    }
}