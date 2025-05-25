using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Utils.Tab;
using World.TheCard;

namespace Popups
{
    public class PopupCardStats : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] Card _card;
        [SerializeField] RectTransform _rectTransformContent;
        [SerializeField] TextMeshProUGUI _txCardName;

        //====================[Element & Class]=====================\\
        [SerializeField] private CardElementUI _element;

        [SerializeField] private CardClassUI _class;

        //====================[Level]=====================\\
        [SerializeField] TextMeshProUGUI _txLevel;
        [SerializeField] private TextMeshProUGUI _txLevelExp;
        [SerializeField] private Image _imgLevelExpFill;

        [SerializeField] CardAttributeUI _cardAttributeUI;

        public async UniTask Init(TabSwitcherWindowModel model = null)
        {
            var theModel = model as PopupCardTabSwitcherWindowModel;
            _card.CardModel = theModel.CardModel;
            var template = await Global.Instance.Get<GameConfig>().GetCardTemplate(theModel.CardModel.TemplateId);

            _txCardName.text = template.Name;
            _element.Init(template.Element);
            _class.Init(template.Class);
            //
            _txLevel.text = theModel.CardModel.Level.GetLevel().ToString();
            _txLevelExp.text =
                $"{theModel.CardModel.Level.GetExpCurrent(false).ToString()}" +
                "<color=black>/</color>" +
                $"{theModel.CardModel.Level.GetExpNext(false).ToString()}";
            _imgLevelExpFill.fillAmount = theModel.CardModel.Level.GetProgress(false) / 100;

            _cardAttributeUI.Init(theModel.CardModel);
        }

        public async UniTask LateInit()
        {
            await _cardAttributeUI.RefreshUI();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransformContent);
        }
    }
}