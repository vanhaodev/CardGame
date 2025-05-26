using System;
using Cysharp.Threading.Tasks;
using Globals;
using Popups;
using UnityEngine;
using World.TheCard;

namespace World.Player.PopupCharacter
{
    public class CardCollectionItem : MonoBehaviour
    {
        [SerializeField] private Card _card;

        /// <summary>
        /// sẽ hiện thêm nút (thêm vào lineup) nếu là lineup selector, pop này show ở tab lineup và là kế thừa của pop collection
        /// </summary>
        private bool _isLineupSelector;

        public async UniTask Set(CardModel cardModel, bool isLineupSelector = false, byte lineupIndex = 0,
            byte lineupSlotIndex = 0, Action OnClose = null)
        {
            _card.CardModel = cardModel;
            _isLineupSelector = isLineupSelector;

            _card.ListenEventOnTouch((c) =>
            {
                PopupCardModel model;

                if (_isLineupSelector)
                {
                    model = new PopupCardEquipModel
                    {
                        CardModel = _card.CardModel,
                        LineupIndex = lineupIndex,
                        SlotIndex = lineupSlotIndex
                    };
                }
                else
                {
                    model = new PopupCardCollectionModel
                    {
                        CardModel = _card.CardModel
                    };
                }

                model.OnClose = OnClose;
                Global.Instance.Get<PopupManager>().ShowCard(
                   model);
            });
        }
    }
}