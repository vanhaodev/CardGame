using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using Utils.Tab;
using World.Player.Character;
using UniRx;
namespace World.Player.PopupCharacter
{
    public class PopupCharacterCardCollection : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] protected CardCollectionItem _prefabCardCollectionItem;
        [SerializeField] protected Transform _parentCardCollectionItem;
        protected List<CardCollectionItem> _items = new List<CardCollectionItem>();
        protected CancellationTokenSource _ctsInit;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private void OnEnable()
        {
            var sub = Global.Instance.Get<CharacterData>().CharacterModel.OnCardCollectionChanged
                .Subscribe(u =>
                {
                    Debug.Log($"OnCardCollectionChanged: {u}");
                    Init().Forget();
                });
            _disposables.Add(sub);
        }

        private void OnDisable()
        {
            _disposables.Dispose();     // Hủy tất cả
            _disposables = new CompositeDisposable(); // Reset
        }

        public virtual async UniTask Init(TabSwitcherWindowModel model = null)
        {
            _ctsInit?.Cancel();
            _ctsInit = new CancellationTokenSource();

            var charData = Global.Instance.Get<CharacterData>();
            var cardCollection = charData.CharacterModel.CardCollection;
            var cards = cardCollection.Cards;

            // Đảm bảo danh sách đủ số lượng
            while (_items.Count < cards.Count)
            {
                var newItem = Instantiate(_prefabCardCollectionItem, _parentCardCollectionItem);
                newItem.gameObject.SetActive(false); // tránh nháy khi chưa Set
                _items.Add(newItem);
            }

            var tasks = new List<UniTask>();

            for (int i = 0; i < _items.Count; i++)
            {
                if (i < cards.Count)
                {
                    var cardModel = cards[i];
                    var item = _items[i];

                    item.gameObject.SetActive(true);
                    tasks.Add(item.Set(cardModel));
                }
                else
                {
                    // Ẩn các item dư
                    _items[i].gameObject.SetActive(false);
                }
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsInit.Token);
        }


        public UniTask LateInit()
        {
            return UniTask.CompletedTask;
        }
    }
}