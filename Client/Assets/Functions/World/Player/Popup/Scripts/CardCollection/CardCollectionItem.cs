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

       private void Awake()
       {
           _card.ListenEventOnTouch((c) =>
           {
               Global.Instance.Get<PopupManager>().ShowCard(_card.CardModel);
           });
       }
       
       public async UniTask Set(CardModel cardModel)
       {
           _card.CardModel = cardModel;
       }
    }

}