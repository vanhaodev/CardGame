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
        [SerializeField] CardCollectionItem _prefabCardCollectionItem;
        [SerializeField] Transform _parentCardCollectionItem;
        CancellationTokenSource _ctsInit;
        /// <summary>
        /// EXCEPTION sẽ hiện thêm nút (thêm vào lineup) nếu là lineup selector, pop này show ở tab lineup và là kế thừa của pop collection
        /// </summary>
        [SerializeField] private bool _isLineupSelector;
        public async UniTask Init()
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
                tasks.Add(cardCollectionItem.Set(cardModel, _isLineupSelector));
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsInit.Token);
        }
    }
}