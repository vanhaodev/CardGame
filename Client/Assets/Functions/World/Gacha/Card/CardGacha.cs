using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Globals;
using Popups;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private GameObject _objShadow;
        UnityAction _onOpen;

        public bool IsOpened()
        {
            return !_objCardBack.activeSelf;
        }

        public void ShowShadow(bool isShow)
        {
            _objShadow.SetActive(isShow);
        }
        public void InitCard(CardModel cardModel, UnityAction onOpen)
        {
            Init();
            _receivedCardModel = cardModel;
            _onOpen = onOpen;
        }

        public void InitItem(ItemModel itemModel, uint itemQuantity, UnityAction onOpen)
        {
            Init();
            _receivedItemModel = itemModel;
            _receivedItemQuantity = itemQuantity;
            _onOpen = onOpen;
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
        /// avoid duplicate volume
        /// </summary>
        static bool _isSFXFlipPlaying;

        /// <summary>
        /// card back khi được nhấn
        /// </summary>
        public async void OnFlipOpen()
        {
            if (_receivedCardModel == null && _receivedItemModel == null) return;
            if (!_isSFXFlipPlaying)
            {
                _isSFXFlipPlaying = true;
                Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_GachaCardFlip");
            }
            
            if (_receivedCardModel != null)
            {
                _card.CardModel = _receivedCardModel;
                await UniTask.WaitForSeconds(0.5f);
                _objCardBack.SetActive(false);
                _card.gameObject.SetActive(true);
            }
            else
            {
                await _item.Init(new InventoryItemModel()
                {
                    Item = _receivedItemModel,
                    Quantity = _receivedItemQuantity
                });
                await UniTask.WaitForSeconds(0.5f);
                _objCardBack.SetActive(false);
                _item.gameObject.SetActive(true);
            }

            _isSFXFlipPlaying = false;
            _onOpen?.Invoke();
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