using System.Linq;
using Globals;
using TMPro;
using UnityEngine;
using Utils.Tab;
using World.TheCard;

namespace Popups
{
    public class PopupCard : Popup
    {
        [SerializeField] private TabSwitcher _tab;
        private PopupCardModel _model;
        public void SetupCard(PopupCardModel model)
        {
            _model = model;
            
            Debug.Log(nameof(model));
            _tab.Init(new PopupCardTabSwitcherWindowModel()
            {
                CardModel = _model.CardModel,
            });
        }
    }
}