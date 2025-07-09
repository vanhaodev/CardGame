using System;
using Functions.World.Player.Inventory;
using Utils.Tab;
using World.TheCard;

namespace Popups
{
    public class PopupCardTabSwitcherWindowModel : TabSwitcherWindowModel
    {
        public PopupCardModel PopupCardModel;
        public Action OnClosePopupCard;
        public ItemActionModel ItemActionModel;
    }
}