using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using UnityEngine;
using Utils.Tab;
using World.TheCard;

namespace Popups
{
    public class PopupCardUpgrade : MonoBehaviour, ITabSwitcherWindow
    {
        private CardModel _cardModel;
        private ItemActionModel _itemAction;
        [SerializeField] private CardUpgrade _upgradeLevel;
        [SerializeField] private CardUpgrade _upgradeAwaken;

        public async UniTask Init(TabSwitcherWindowModel model = null)
        {
            var tabSwitcherWindowModel = model as PopupCardTabSwitcherWindowModel;
            _cardModel = tabSwitcherWindowModel.PopupCardModel.CardModel;
            _itemAction = tabSwitcherWindowModel.ItemActionModel;
        }

        public async UniTask LateInit()
        {
        }

        //---------------
        public void OpenUpgradeLevel()
        {
            _upgradeLevel.SetData(_cardModel, _itemAction);
            _upgradeLevel.gameObject.SetActive(true);
        }

        public void OpenUpgradeAwaken()
        {
            _upgradeAwaken.SetData(_cardModel, _itemAction);
            _upgradeAwaken.gameObject.SetActive(true);
        }
    }
}