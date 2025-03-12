using System.Linq;
using Globals;
using TMPro;
using UnityEngine;
using World.Card;

namespace Popup
{
    public class PopupCard : Popup
    {
        [SerializeField] Card _card;
        [SerializeField] private GameObject _objInfo;
        [SerializeField] private TextMeshProUGUI _txInfo;
        public void Setup(CardModel cardModel)
        {
            _card.CardModel = cardModel;
        }

        public async void OpenInfo()
        {
            var temp = await Global.Instance.Get<CardLoader>().GetCardTemplate(_card.CardModel.TemplateId);
            _txInfo.text = $"{temp.Name}\n" +
                           $"{temp.History}\n" +
                           $"{string.Join("\n", _card.CardModel.CalculatedAttributes.Select(a => $"{a.Type}: {a.Value}"))}";
            _objInfo.SetActive(true);
        }

        public void CloseInfo()
        {
            _txInfo.text = "";
            _objInfo.SetActive(false);
        }
    }
}