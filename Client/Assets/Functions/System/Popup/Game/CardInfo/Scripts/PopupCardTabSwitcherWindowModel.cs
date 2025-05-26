using System;
using Utils.Tab;
using World.TheCard;

namespace Popups
{
    public class PopupCardTabSwitcherWindowModel : TabSwitcherWindowModel
    {
        public PopupCardModel PopupCardModel;
        public Action OnClosePopupCard;
    }
}