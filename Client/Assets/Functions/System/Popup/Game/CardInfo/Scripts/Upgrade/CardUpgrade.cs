using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using UnityEngine;
using World.TheCard;

namespace Popups
{
    public abstract class CardUpgrade : MonoBehaviour
    {
        [SerializeField] protected GameObject _objBlockBehind;
        protected CardModel _cardModel;
        protected ItemActionModel _itemAction;

        public virtual async UniTask SetData(CardModel cardModel, ItemActionModel itemAction)
        {
            _cardModel = cardModel;
            _itemAction = itemAction;
            _objBlockBehind?.SetActive(true);
        }

        public virtual void Close()
        {
            _itemAction?.OnClose?.Invoke();
            gameObject.SetActive(false);
            _objBlockBehind?.SetActive(false);
        }
    }
}