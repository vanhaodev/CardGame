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

        private void Awake()
        {
            _card.ListenEventOnTouch((c) => { Global.Instance.Get<PopupManager>().ShowCard(_card.CardModel); });
        }

        public async UniTask Set(CardModel cardModel, bool isLineupSelector)
        {
            _card.CardModel = cardModel;
            _isLineupSelector = isLineupSelector;
        }
    }
}