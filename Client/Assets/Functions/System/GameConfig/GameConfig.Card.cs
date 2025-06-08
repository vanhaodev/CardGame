using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.World.Gacha;
using Globals;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.TheCard;

namespace GameConfigs
{
    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        public IReadOnlyDictionary<ushort, uint> LevelExps { get; private set; }

        public async UniTask InitCard()
        {
            await GetCardGacha(1);
            await GetCardGacha(2);
            await GetCardGacha(3);
            var paths = new[]
            {
                "CardConfigs/CardLevelExps"
            };

            var tasks = new List<UniTask<TextSO>>();
            foreach (var path in paths)
            {
                tasks.Add(Global.Instance.Get<AddressableLoader>().LoadAssetAsync<TextSO>(path).AsUniTask());
            }

            var results = await UniTask.WhenAll(tasks);

            // Deserialize từ JSON dạng tịnh tiến
            var levelExpsList = JsonConvert.DeserializeObject<List<uint>>(results[0].Content);

            // Chuyển List thành Dictionary: key = level, value = required total EXP
            LevelExps = levelExpsList
                .Select((exp, index) => new { Level = (ushort)(index + 1), Exp = exp }) // Index 0 → Level 1
                .ToDictionary(x => x.Level, x => x.Exp);

            /*
             📌 Giải thích:
            Level 1 cần 0 EXP.
            Level 2 cần 100 EXP.
            Level 3 cần tổng 210 EXP (tức lên từ 2 → 3 cần thêm 110).
            ...
            Level 20 cần tổng 5107 EXP.
             */
            // Debug.Log($"Loaded\n" +
            //           $"\n{LevelExps.Count} level exps");
        }

        private ConcurrentDictionary<ushort, CardTemplateModel> _cardTemplates = new();

        public async UniTask<CardTemplateModel> GetCardTemplate(ushort cardTemplateId)
        {
            if (_cardTemplates.TryGetValue(cardTemplateId, out var value))
            {
                return value;
            }

            var cardSO = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<CardTemplateModel>("CardTemplateModels/" + cardTemplateId);
            _cardTemplates.TryAdd(cardTemplateId, cardSO);
            return cardSO;
        }

        /// <summary>
        /// sprite of this card
        /// </summary>
        private ConcurrentDictionary<ushort, ConcurrentDictionary<string, Sprite>> _loadedCardSprites = new();

        public async UniTask<Sprite> GetCardSprite(CardModel cardModel, string skinName = "" /*default is null*/)
        {
            if (cardModel.Star < 1) throw new System.Exception("Card star must be >= 1");
            var cardTemplateId = cardModel.TemplateId;
            var key = $"{cardModel.Star}";

            // Đảm bảo ConcurrentDictionary con tồn tại
            var value = _loadedCardSprites.GetOrAdd(cardTemplateId, _ => new ConcurrentDictionary<string, Sprite>());

            // Nếu key đã tồn tại, trả về ngay
            if (value.TryGetValue(key, out var image))
            {
                return image;
            }

            // Tải sprite mới
            var newSprite = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<Sprite>(
                    $"CardSprites/{cardTemplateId}{(skinName.Length > 0 ? $"/{skinName}" : "")}/{key}.png");
            // Thêm vào dictionary nếu chưa tồn tại (tránh lỗi "key exists")
            value.TryAdd(key, newSprite);

            return newSprite;
        }

        public (ushort level, float progressPercent, uint expCurrent, uint expNext) GetLevelProgressAndNextExp(uint exp)
        {
            if (LevelExps == null || LevelExps.Count == 0)
                return (1, 0f, 0, 0); // default level 1 nếu chưa có data

            var ordered = LevelExps.OrderBy(kvp => kvp.Key).ToList();
            // Debug.Log($"Level exps: {JsonConvert.SerializeObject(ordered)}");
            for (int i = 0; i < ordered.Count; i++)
            {
                ushort level = ordered[i].Key;
                uint requiredExp = ordered[i].Value;

                if (exp < requiredExp)
                {
                    if (i == 0)
                    {
                        // exp thấp hơn mốc đầu tiên => level 1
                        return (1, 0f, exp, requiredExp);
                    }

                    ushort prevLevel = ordered[i - 1].Key;
                    uint prevExp = ordered[i - 1].Value;

                    uint range = requiredExp - prevExp;
                    uint currentProgress = exp - prevExp;

                    float progress = range > 0 ? (float)currentProgress / range * 100f : 0f;
                    return (prevLevel, progress, currentProgress, range);
                }
            }

            // Nếu exp >= max mốc
            ushort maxLevel = ordered.Last().Key;
            return (maxLevel, 100f, 0, 0);
        }
    }

    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        [SerializeReference] private List<GachaCardModel> _gachaCard1; //gacha thường
        [SerializeReference] private List<GachaCardModel> _gachaCard2; //gacha trung cấp
        [SerializeReference] private List<GachaCardModel> _gachaCard3; //gacha cao cấp
        /// <summary>
        /// 1: thường <br/>
        /// 2: trung cấp <br/>
        /// 3: cao cấp <br/>
        /// </summary>
        /// <param name="type"></param>
        public async UniTask<List<GachaCardModel>> GetCardGacha(byte type)
        {
            switch (type)
            {
                case 1:
                {
                    if (_gachaCard1 == null || _gachaCard1.Count == 0)
                    {
                        var json1 = await Global.Instance.Get<AddressableLoader>()
                            .LoadAssetAsync<TextSO>("CardConfigs/GachaCard1");
                        _gachaCard1 = JsonConvert.DeserializeObject<List<GachaCardModel>>(json1.Content);
                    }

                    return _gachaCard1;
                }
                case 2:
                {
                    if (_gachaCard2 == null || _gachaCard2.Count == 0)
                    {
                        var json2 = await Global.Instance.Get<AddressableLoader>()
                            .LoadAssetAsync<TextSO>("CardConfigs/GachaCard2");
                        _gachaCard2 = JsonConvert.DeserializeObject<List<GachaCardModel>>(json2.Content);
                    }

                    return _gachaCard2;
                }
                case 3:
                {
                    if (_gachaCard3 == null || _gachaCard3.Count == 0)
                    {
                        var json3 = await Global.Instance.Get<AddressableLoader>()
                            .LoadAssetAsync<TextSO>("CardConfigs/GachaCard3");
                        _gachaCard3 = JsonConvert.DeserializeObject<List<GachaCardModel>>(json3.Content);
                    }

                    return _gachaCard3;
                }
            }

            return null;
        }
    }
}