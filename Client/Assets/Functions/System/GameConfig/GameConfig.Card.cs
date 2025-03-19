using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Globals;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.Card;

namespace GameConfig
{
    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        public IReadOnlyDictionary<ushort, uint> LevelExps { get; private set; }
        public async UniTask InitCard()
        {
            var paths = new[]
            {
                "ItemTemplates/LevelExpsSO.asset"
            };

            var tasks = new List<UniTask<TextSO>>();
            foreach (var path in paths)
            {
                tasks.Add(Global.Instance.Get<AddressableLoader>().LoadAssetAsync<TextSO>(path).AsUniTask());
            }

            var results = await UniTask.WhenAll(tasks);

            // Deserialize từ JSON List<>
            var levelExpsList = JsonConvert.DeserializeObject<List<uint>>(results[0].Content);

            // Chuyển List thành Dictionary
            LevelExps = levelExpsList
                .Select((exp, index) => new { Level = (ushort)(index + 2), Exp = exp }) // Level bắt đầu từ 2
                .ToDictionary(x => x.Level, x => x.Exp);

            Debug.Log($"Loaded\n" +
                      $"\n{LevelExps.Count} level exps");
        }
        private ConcurrentDictionary<ushort, CardTemplateModel> _cardTemplates = new();

        public async UniTask<CardTemplateModel> GetCardTemplate(ushort cardTemplateId)
        {
            if (_cardTemplates.TryGetValue(cardTemplateId, out var value))
            {
                return value;
            }

            var textSO = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<TextSO>("CardTemplates/" + cardTemplateId + ".asset");
            var newCard = JsonConvert.DeserializeObject<CardTemplateModel>(textSO.Content);
            _cardTemplates.TryAdd(cardTemplateId, newCard);
            return newCard;
        }

        /// <summary>
        /// sprite of this card
        /// </summary>
        private ConcurrentDictionary<ushort, ConcurrentDictionary<string, Sprite>> _loadedCardSprites = new();

        public async UniTask<Sprite> GetCardSprite(CardModel cardModel)
        {
            var cardTemplateId = cardModel.TemplateId;
            var key = $"{cardTemplateId}_{cardModel.Rank}";

            // Đảm bảo ConcurrentDictionary con tồn tại
            var value = _loadedCardSprites.GetOrAdd(cardTemplateId, _ => new ConcurrentDictionary<string, Sprite>());

            // Nếu key đã tồn tại, trả về ngay
            if (value.TryGetValue(key, out var image))
            {
                return image;
            }

            // Tải sprite mới
            var newSprite = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<Sprite>("CardSprites/" + key + ".png");

            // Thêm vào dictionary nếu chưa tồn tại (tránh lỗi "key exists")
            value.TryAdd(key, newSprite);

            return newSprite;
        }

    }
}