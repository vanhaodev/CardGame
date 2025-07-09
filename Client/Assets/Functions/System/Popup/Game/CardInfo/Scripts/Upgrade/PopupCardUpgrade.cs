using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Tab;
using World.TheCard;

namespace Popups
{
    public class PopupCardUpgrade : MonoBehaviour, ITabSwitcherWindow
    {
        private CardModel _cardModel;
        [SerializeField] private CardUpgrade _upgradeLevel;
        [SerializeField] private CardUpgrade _upgradeAwaken;
        public async UniTask Init(TabSwitcherWindowModel model = null)
        {
            var tabSwitcherWindowModel = model as PopupCardTabSwitcherWindowModel;
            _cardModel = tabSwitcherWindowModel.PopupCardModel.CardModel;
        }

        public async UniTask LateInit()
        {

        }
        //---------------
        public void OpenUpgradeLevel()
        {
            _upgradeLevel.SetCard(_cardModel);
            _upgradeLevel.gameObject.SetActive(true);
        }
        public void OpenUpgradeAwaken()
        {
            _upgradeAwaken.SetCard(_cardModel);
            _upgradeAwaken.gameObject.SetActive(true);
        }
    }
}