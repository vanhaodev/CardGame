using Functions.World.Player.Inventory;
using Globals;
using Popups;
using UnityEngine;
using World.Player.PopupCharacter;
using World.TheCard;

namespace Functions.World.Gacha
{
    public class CardGacha : MonoBehaviour
    {
        [SerializeField] private GameObject _objCardBack;
        [SerializeField] private Card _card;
        [SerializeField] private InventoryItemUI _item;
        [SerializeField] private CardModel _receivedCardModel;
        [SerializeField] private ItemModel _receivedItemModel;
        [SerializeField] private uint _receivedItemQuantity;
        public void InitCard(CardModel cardModel)
        {
            Init();
            _receivedCardModel = cardModel;
        }

        public void InitItem(ItemModel itemModel, uint itemQuantity)
        {
            Init();
            _receivedItemModel = itemModel;
            _receivedItemQuantity = itemQuantity;
        }
        private void Init()
        {
            _receivedCardModel = null;
            _receivedItemModel = null;
            _receivedItemQuantity = 0;
            _card.CardModel = null;
            _item.Clear();
            _objCardBack.SetActive(true);
            _card.gameObject.SetActive(false);
            _item.gameObject.SetActive(false);
        }
        /// <summary>
        /// card back khi được nhấn
        /// </summary>
        public async void OnFlipOpen()
        {
            _objCardBack.SetActive(false);
            if (_receivedCardModel != null)
            {
                _card.CardModel = _receivedCardModel;
                _card.gameObject.SetActive(true);
            }
            else
            {
                await _item.Init(new InventoryItemModel()
                {
                    Item = _receivedItemModel,
                    Quantity = _receivedItemQuantity
                });
                _item.gameObject.SetActive(true);
            }
        }

        public void OnShowCardInfo()
        {
            Global.Instance.Get<PopupManager>().ShowCard(new PopupCardBattleModel()
            {
                CardModel = _card.CardModel
            });
        }
    }
}