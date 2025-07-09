using UnityEngine;
using World.TheCard;

namespace Popups
{
    public abstract class CardUpgrade : MonoBehaviour
    {
        [SerializeField] private GameObject _objBlockBehind;
        private CardModel _cardModel;

        public virtual void SetCard(CardModel cardModel)
        {
            _cardModel = cardModel;
            _objBlockBehind?.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            _objBlockBehind?.SetActive(false);
        }
    }
}