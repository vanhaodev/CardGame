using Cysharp.Threading.Tasks;
using UnityEngine;
using World.TheCard;

namespace World.Player.PopupCharacter
{
    public class CardCollectionItem : MonoBehaviour
    {
       [SerializeField] private Card _card;

       public async UniTask Set(CardModel cardModel)
       {
           _card.CardModel = cardModel;
       }
    }

}