using System.Linq;
using Globals;
using TMPro;
using UnityEngine;
using World.TheCard;

namespace Popups
{
    public class PopupCard : Popup
    {
        [SerializeField] Card _card;
        private PopupCardModel _model;
        public void SetupCard(PopupCardModel model)
        {
            _card.CardModel = model.CardModel;
            _model = model;
            
            Debug.Log(nameof(model));
        }
    }
}