using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using Utils.Tab;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    //Có tính năng tương tự base nhưng dành cho việc chọn card vào lineup thay cho collection chỉ có thể xem
    public class PopupCharacterCardCollectionSelector : PopupCharacterCardCollection
    {
        public byte LineupIndex;
        public byte SlotIndex;
        public override async UniTask Init(TabSwitcherWindowModel model = null)
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
                tasks.Add(cardCollectionItem.Set(cardModel, true, LineupIndex, SlotIndex, OnClose: Close));
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsInit.Token);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}