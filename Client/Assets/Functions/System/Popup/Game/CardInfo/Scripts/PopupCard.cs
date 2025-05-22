using System.Linq;
using Globals;
using TMPro;
using UnityEngine;
using World.TheCard;

namespace Popups
{
    public enum PopupCardDisplayType
    {
        /// <summary>
        /// Có thêm nút tháo khỏi lineup
        /// </summary>
        LineupEquip,
        LineupUnequip,
        /// <summary>
        /// Các nút chi tiết như nâng cấp, sao...
        /// </summary>
        Collection,
        /// <summary>
        /// Chỉ xem được thông tin
        /// </summary>
        Battle
    }

    public class PopupCard : Popup
    {
        [SerializeField] Card _card;
        [SerializeField] private GameObject _objInfo;
        [SerializeField] private TextMeshProUGUI _txInfo;
        private PopupCardDisplayType _displayType;
        public void SetupCard(CardModel cardModel, PopupCardDisplayType displayType)
        {
            _card.CardModel = cardModel;
            _displayType = displayType;
        }

        public async void OpenInfo()
        {
            var temp = await Global.Instance.Get<GameConfigs.GameConfig>().GetCardTemplate(_card.CardModel.TemplateId);
            _txInfo.text = $"{temp.Name} {_displayType}\n" +
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