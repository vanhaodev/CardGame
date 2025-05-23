using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using Utils.Tab;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterCardCollection : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] protected CardCollectionItem _prefabCardCollectionItem;
        [SerializeField] protected Transform _parentCardCollectionItem;
        protected CancellationTokenSource _ctsInit;
        public virtual async UniTask Init()
        {
            _ctsInit?.Cancel();
            _ctsInit = new CancellationTokenSource();
            foreach (Transform child in _parentCardCollectionItem)
            {
                Destroy(child.gameObject);
            }

            var charData = Global.Instance.Get<CharacterData>();
            var cardCollection = charData.CharacterModel.CardCollection;
            var tasks = new List<UniTask>();
            for (int i = 0; i < cardCollection.Cards.Count; i++)
            {
                var cardModel = cardCollection.Cards[i];
                var cardCollectionItem = Instantiate(_prefabCardCollectionItem, _parentCardCollectionItem);
                tasks.Add(cardCollectionItem.Set(cardModel, false));
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsInit.Token);
        }
    }
}