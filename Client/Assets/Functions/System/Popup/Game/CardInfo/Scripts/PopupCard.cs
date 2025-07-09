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
                PopupCardModel = model,
                OnClosePopupCard = (() =>
                {
                    Close();
                    model.OnClose?.Invoke();
                }),
                ItemActionModel = null
            });

            switch (model)
            {
                case PopupCardEquipModel equip:
                case PopupCardUnequipModel unequip:
                case PopupCardCollectionModel collection:
                {
                    _tab.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(true));
                    break;
                }
                case PopupCardBattleModel battle:
                {
                    _tab.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(false));
                    _tab.Tabs[0].TabSwitcherButton.SetButtonActive(true);
                    break;
                }
            }
        }

        private void SetupFeatureTabs()
        {
        }
    }
}