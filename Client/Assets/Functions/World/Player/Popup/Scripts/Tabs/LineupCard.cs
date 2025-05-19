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
            _slotIndex = slotIndex;
            _txSlotIndex.text = slotIndex.ToString();
            var cardLineups = Global.Instance.Get<CharacterData>().CharacterModel.CardLineups;
            if (lineupIndex - 1 >= 0 &&
                lineupIndex - 1 < cardLineups.Count &&
                cardLineups[lineupIndex - 1] is { Cards: not null } lineup &&
                lineup.Cards.TryGetValue(_slotIndex, out var lineupCard))
            {
                var cardTemp = await Global.Instance.Get<GameConfig>().GetCardTemplate(lineupCard.TemplateId);
                _objEmpty.SetActive(false);
                _card.gameObject.SetActive(true);
                _txCardName.text = cardTemp.Name;
            }
            else
            {
                _objEmpty.SetActive(true);
                _card.gameObject.SetActive(false);
                _txCardName.text = "";
            }
        }
        
    }
}