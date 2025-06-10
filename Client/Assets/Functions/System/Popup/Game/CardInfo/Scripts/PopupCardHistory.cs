using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using TMPro;
using UnityEngine;
using Utils.Tab;

namespace Popups
{
    public class PopupCardHistory : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] private TextMeshProUGUI _txContent;
        public async UniTask Init(TabSwitcherWindowModel model = null)
        {
            var tabSwitcherWindowModel = model as PopupCardTabSwitcherWindowModel;
            var cardModel = tabSwitcherWindowModel.PopupCardModel.CardModel;
            var temp = await Global.Instance.Get<GameConfig>().GetCardTemplate(cardModel.TemplateId);
            _txContent.text = temp.History;
        }

        public UniTask LateInit()
        {
            return UniTask.CompletedTask;
        }
    }

}