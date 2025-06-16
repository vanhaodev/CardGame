using System;
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

        private ConcurrentDictionary<uint, CardTemplateModel> _cardTemplates = new();

        public async UniTask<CardTemplateModel> GetCardTemplate(uint cardTemplateId)
        {
            if (_cardTemplates.TryGetValue(cardTemplateId, out var value))
            {
                return value;
            }

            var cardSO = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<CardTemplateModel>("CardTemplates/" + cardTemplateId);
            _cardTemplates.TryAdd(cardTemplateId, cardSO);
            return cardSO;
        }

        /// <summary>
        /// sprite of this card
        /// </summary>
        private ConcurrentDictionary<uint, ConcurrentDictionary<string, Sprite>> _loadedCardSprites = new();

        public async UniTask<Sprite> GetCardShardIcon(uint cardTemplateId)
        {
            var key = $"CardSprites/{cardTemplateId}";
            // Đảm bảo ConcurrentDictionary con tồn tại
            var value = _loadedCardSprites.GetOrAdd(cardTemplateId, _ => new ConcurrentDictionary<string, Sprite>());

            // Nếu key đã tồn tại, trả về ngay
            if (value.TryGetValue(key, out var image))
            {
                return image;
            }

            var newSprite = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<Sprite>(key);
            // Thêm vào dictionary nếu chưa tồn tại (tránh lỗi "key exists")
            value.TryAdd(key, newSprite);

            return newSprite;
        }

        public async UniTask<Sprite> GetCardSprite(CardModel cardModel, uint skinId = 0 /*default is null*/)
        {
            var cardTemplateId = cardModel.TemplateId;
            var key = $"CardSprites/{cardTemplateId}{(skinId > 0 ? $"_{skinId}" : "")}";

            // Đảm bảo ConcurrentDictionary con tồn tại
            var value = _loadedCardSprites.GetOrAdd(cardTemplateId, _ => new ConcurrentDictionary<string, Sprite>());

            // Nếu key đã tồn tại, trả về ngay
            if (value.TryGetValue(key, out var image))
            {
                return image;
            }

            // Tải sprite mới
            var newSprite = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<Sprite>(key);
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

        /// <summary>
        /// Tăng chỉ số theo level card: levelValue = baseValue + (baseValue * levelPercentBonus / 100f);
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float CardLevelAttributePercentBonus(ushort level)
        {
            switch (level)
            {
                case 1: return 0f;
                case 2: return 2f;
                case 3: return 4f;
                case 4: return 6f;
                case 5: return 8f;
                case 6: return 10f;
                case 7: return 12f;
                case 8: return 14;
                case 9: return 16;
                case 10: return 18;
                case 11: return 20;
                case 12: return 22;
                case 13: return 24;
                case 14: return 26;
                case 15: return 28;
                case 16: return 30;
                case 17: return 32;
                case 18: return 34;
                case 19: return 36;
                case 20: return 38;
                case 21: return 40;
                case 22: return 42;
                case 23: return 44;
                case 24: return 46;
                case 25: return 48;
                case 26: return 50;
                case 27: return 52;
                case 28: return 54;
                case 29: return 56;
                case 30: return 58;
                case 31: return 60;
                case 32: return 62;
                case 33: return 64;
                case 34: return 66;
                case 35: return 68;
                case 36: return 70;
                case 37: return 72;
                case 38: return 74;
                case 39: return 76;
                case 40: return 78;
                case 41: return 80;
                case 42: return 100;
                case 43: return 105;
                case 44: return 110;
                case 45: return 115;
                case 46: return 120;
                case 47: return 125;
                case 48: return 130;
                case 49: return 135;
                case 50: return 140;
                default: return 0;
            }
        }

        public float CardStarAttributePercentBonus(byte star)
        {
            switch (star)
            {
                case 1: return 0f;
                case 2: return 100f;
                case 3: return 200f;
                case 4: return 300f;
                case 5: return 400f;
                default: return 0f;
            }
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
        public async UniTask<List<GachaCardModel>> GetCardGacha(int type)
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