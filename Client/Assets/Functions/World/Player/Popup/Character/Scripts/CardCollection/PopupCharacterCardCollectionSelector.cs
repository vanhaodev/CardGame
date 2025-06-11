using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.Utilities;
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

            var charData = Global.Instance.Get<CharacterData>();

            HashSet<uint> idsExistsInCurrentLineup = new HashSet<uint>();
            HashSet<uint> templateIdsExistsInCurrentLineup = new HashSet<uint>();
            if (charData.CharacterModel.CardLineups.Count > LineupIndex)
            {
                idsExistsInCurrentLineup =
                    new HashSet<uint>(charData.CharacterModel.CardLineups[LineupIndex].Cards.Values);
                for (int i = 0; i < charData.CharacterModel.CardCollection.Cards.Count; i++)
                {
                    if (idsExistsInCurrentLineup.Contains(charData.CharacterModel.CardCollection.Cards[i].Id))
                    {
                        templateIdsExistsInCurrentLineup.Add(charData.CharacterModel.CardCollection.Cards[i]
                            .TemplateId);
                    }
                }
            }

            var cards = charData.CharacterModel.CardCollection.Cards
                .Where(i => !idsExistsInCurrentLineup.Contains(i.Id) &&
                            !templateIdsExistsInCurrentLineup.Contains(i.TemplateId))
                .ToList();


            // Tạo thêm nếu chưa đủ item
            while (_items.Count < cards.Count)
            {
                var newItem = Instantiate(_prefabCardCollectionItem, _parentCardCollectionItem);
                newItem.gameObject.SetActive(false);
                _items.Add(newItem);
            }

            var tasks = new List<UniTask>();

            for (int i = 0; i < _items.Count; i++)
            {
                var cardCollectionItem = _items[i]; // ✅ rõ ràng, không lẫn với _items

                if (i < cards.Count)
                {
                    if (!cardCollectionItem.transform.IsChildOf(_parentCardCollectionItem))
                        cardCollectionItem.transform.SetParent(_parentCardCollectionItem, false);

                    cardCollectionItem.gameObject.SetActive(true);
                    tasks.Add(cardCollectionItem.Set(cards[i], true, LineupIndex, SlotIndex, OnClose: Close));
                }
                else
                {
                    cardCollectionItem.gameObject.SetActive(false);
                }
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsInit.Token);
        }


        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}