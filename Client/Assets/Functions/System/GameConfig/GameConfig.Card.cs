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

        public Dictionary<AttributeType, int> CardLevelAttributeBonus(ClassType classType, ushort level)
        {
            switch (classType)
            {
                case ClassType.Crusher:
                    return Crusher();
                case ClassType.Assassin:
                    return Assassin();
                case ClassType.Demolisher:
                    return Demolisher();
                case ClassType.Tactician:
                    return Tactician();
                case ClassType.Guardian:
                    return Guardian();
                case ClassType.Medic:
                    return Medic();
                default:
                    throw new ArgumentOutOfRangeException(nameof(classType), classType, null);
            }

            Dictionary<AttributeType, int> Crusher()
            {
                return new Dictionary<AttributeType, int>()
                {
                    { AttributeType.HpMax, 80 * level },
                    { AttributeType.Attack, 8 * level },
                    { AttributeType.Defense, 8 * level },
                    { AttributeType.Speed, 2 * level },
                    { AttributeType.CriticalRate, 8 * level },
                    { AttributeType.CriticalDamage, 10 * level },
                };
            }

            Dictionary<AttributeType, int> Assassin()
            {
                return new Dictionary<AttributeType, int>()
                {
                    { AttributeType.HpMax, 50 * level },
                    { AttributeType.Attack, 15 * level },
                    { AttributeType.Defense, 4 * level },
                    { AttributeType.Speed, 3 * level },
                    { AttributeType.ArmorPenetrationRate, 9 * level },
                    { AttributeType.ArmorPenetrationDamage, 7 * level },
                };
            }

            Dictionary<AttributeType, int> Demolisher()
            {
                return new Dictionary<AttributeType, int>()
                {
                    { AttributeType.HpMax, 60 * level },
                    { AttributeType.Attack, 6 * level },
                    { AttributeType.Defense, 6 * level },
                    { AttributeType.Speed, 1 * level },
                    { AttributeType.UPRegeneration, 5 * level },
                };
            }

            Dictionary<AttributeType, int> Tactician()
            {
                return new Dictionary<AttributeType, int>()
                {
                    { AttributeType.HpMax, 40 * level },
                    { AttributeType.Attack, 8 * level },
                    { AttributeType.Defense, 4 * level },
                    { AttributeType.Speed, 2 * level },
                    { AttributeType.DodgeRate, 4 * level },
                    { AttributeType.DodgeDamage, 5 * level },
                    { AttributeType.UPRegeneration, 5 * level },
                };
            }

            Dictionary<AttributeType, int> Guardian()
            {
                return new Dictionary<AttributeType, int>()
                {
                    { AttributeType.HpMax, 145 * level },
                    { AttributeType.Attack, 5 * level },
                    { AttributeType.Defense, 12 * level },
                    { AttributeType.Speed, 1 * level },
                    { AttributeType.DodgeRate, 6 * level },
                    { AttributeType.DodgeDamage, 8 * level },
                    { AttributeType.EffectResistRate, 4 * level },
                    { AttributeType.HealingReceived, 4 * level },
                };
            }

            Dictionary<AttributeType, int> Medic()
            {
                return new Dictionary<AttributeType, int>()
                {
                    { AttributeType.HpMax, 145 * level },
                    { AttributeType.Attack, 5 * level },
                    { AttributeType.Defense, 12 * level },
                    { AttributeType.Speed, 1 * level },
                    { AttributeType.DodgeRate, 2 * level },
                    { AttributeType.DodgeDamage, 3 * level },
                    { AttributeType.OutgoingHealing, 7 * level },
                    { AttributeType.EffectHitRate, 4 * level },
                };
            }
        }

        /// <summary>
        /// Không cộng dồn, star nào thì lấy của star đó thôi
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="star"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Dictionary<AttributeType, int> CardStarAttributeBonus(ClassType classType, byte star)
        {
            switch (classType)
            {
                case ClassType.Crusher:
                    return Crusher();
                case ClassType.Assassin:
                    return Assassin();
                case ClassType.Demolisher:
                    return Demolisher();
                case ClassType.Tactician:
                    return Tactician();
                case ClassType.Guardian:
                    return Guardian();
                case ClassType.Medic:
                    return Medic();
                default:
                    throw new ArgumentOutOfRangeException(nameof(classType), classType, null);
            }

            Dictionary<AttributeType, int> Crusher()
            {
                switch (star)
                {
                    default: return new();
                    case 1:
                        return new()
                        {
                            { AttributeType.HpMax, 350 },
                            { AttributeType.Attack, 40 },
                            { AttributeType.Defense, 40 },
                            { AttributeType.Speed, 65 },
                        };
                    case 2:
                        return new()
                        {
                            { AttributeType.HpMax, 650 },
                            { AttributeType.Attack, 100 },
                            { AttributeType.Defense, 100 },
                            { AttributeType.Speed, 150 },
                            { AttributeType.EffectResistRate, 300 },
                            { AttributeType.ToughnessHitRate, 500 },
                            { AttributeType.CriticalRate, 300 },
                            { AttributeType.CriticalDamage, 500 },
                        };
                    case 3:
                        return new()
                        {
                            { AttributeType.HpMax, 980 },
                            { AttributeType.Attack, 180 },
                            { AttributeType.Defense, 180 },
                            { AttributeType.Speed, 220 },
                            { AttributeType.EffectResistRate, 550 },
                            { AttributeType.ToughnessHitRate, 700 },
                            { AttributeType.CriticalRate, 550 },
                            { AttributeType.CriticalDamage, 750 },
                        };
                    case 4:
                        return new()
                        {
                            { AttributeType.HpMax, 1600 },
                            { AttributeType.Attack, 250 },
                            { AttributeType.Defense, 250 },
                            { AttributeType.Speed, 350 },
                            { AttributeType.EffectResistRate, 700 },
                            { AttributeType.ToughnessHitRate, 850 },
                            { AttributeType.CriticalRate, 700 },
                            { AttributeType.CriticalDamage, 1000 },
                        };
                    case 5:
                        return new()
                        {
                            { AttributeType.HpMax, 2900 },
                            { AttributeType.Attack, 400 },
                            { AttributeType.Defense, 400 },
                            { AttributeType.Speed, 550 },
                            { AttributeType.EffectResistRate, 1000 },
                            { AttributeType.ToughnessHitRate, 1100 },
                            { AttributeType.CriticalRate, 1500 },
                            { AttributeType.CriticalDamage, 1800 },
                            { AttributeType.UPRegeneration, 500 },
                        };
                }
            }

            Dictionary<AttributeType, int> Assassin()
            {
                switch (star)
                {
                    default: return new();
                    case 1:
                        return new()
                        {
                            { AttributeType.HpMax, 210 },
                            { AttributeType.Attack, 65 },
                            { AttributeType.Defense, 25 },
                            { AttributeType.Speed, 65 },
                        };
                    case 2:
                        return new()
                        {
                            { AttributeType.HpMax, 810 },
                            { AttributeType.Attack, 150 },
                            { AttributeType.Defense, 50 },
                            { AttributeType.Speed, 150 },
                            { AttributeType.ArmorPenetrationRate, 300 },
                            { AttributeType.ArmorPenetrationDamage, 500 },
                        };
                    case 3:
                        return new()
                        {
                            { AttributeType.HpMax, 1250 },
                            { AttributeType.Attack, 250 },
                            { AttributeType.Defense, 120 },
                            { AttributeType.Speed, 240 },
                            { AttributeType.ToughnessHitRate, 900 },
                            { AttributeType.ArmorPenetrationRate, 500 },
                            { AttributeType.ArmorPenetrationDamage, 800 },
                        };
                    case 4:
                        return new()
                        {
                            { AttributeType.HpMax, 1650 },
                            { AttributeType.Attack, 400 },
                            { AttributeType.Defense, 190 },
                            { AttributeType.Speed, 370 },
                            { AttributeType.ToughnessHitRate, 1000 },
                            { AttributeType.ArmorPenetrationRate, 750 },
                            { AttributeType.ArmorPenetrationDamage, 1000 },
                        };
                    case 5:
                        return new()
                        {
                            { AttributeType.HpMax, 2500 },
                            { AttributeType.Attack, 700 },
                            { AttributeType.Defense, 320 },
                            { AttributeType.Speed, 600 },
                            { AttributeType.ToughnessHitRate, 1100 },
                            { AttributeType.CriticalRate, 1300 },
                            { AttributeType.ArmorPenetrationRate, 1000 },
                            { AttributeType.ArmorPenetrationDamage, 1400 },
                        };
                }
            }

            Dictionary<AttributeType, int> Demolisher()
            {
                switch (star)
                {
                    default:
                        return new();
                    case 1:
                        return new()
                        {
                            { AttributeType.HpMax, 300 },
                            { AttributeType.Attack, 40 },
                            { AttributeType.Defense, 40 },
                            { AttributeType.Speed, 20 },
                            { AttributeType.CriticalRate, 300 },
                            { AttributeType.CriticalDamage, 500 },
                            { AttributeType.UPRegeneration, 200 },
                        };
                    case 2:
                        return new()
                        {
                            { AttributeType.HpMax, 520 },
                            { AttributeType.Attack, 100 },
                            { AttributeType.Defense, 100 },
                            { AttributeType.Speed, 80 },
                            { AttributeType.CriticalRate, 550 },
                            { AttributeType.CriticalDamage, 750 },
                            { AttributeType.UPRegeneration, 300 },
                        };
                    case 3:
                        return new()
                        {
                            { AttributeType.HpMax, 880 },
                            { AttributeType.Attack, 150 },
                            { AttributeType.Defense, 150 },
                            { AttributeType.Speed, 120 },
                            { AttributeType.CriticalRate, 700 },
                            { AttributeType.CriticalDamage, 1000 },
                            { AttributeType.UPRegeneration, 500 },
                        };
                    case 4:
                        return new()
                        {
                            { AttributeType.HpMax, 1600 },
                            { AttributeType.Attack, 250 },
                            { AttributeType.Defense, 250 },
                            { AttributeType.Speed, 180 },
                            { AttributeType.CriticalRate, 700 },
                            { AttributeType.CriticalDamage, 1000 },
                            { AttributeType.UPRegeneration, 650 },
                        };
                    case 5:
                        return new()
                        {
                            { AttributeType.HpMax, 2700 },
                            { AttributeType.Attack, 350 },
                            { AttributeType.Defense, 350 },
                            { AttributeType.Speed, 290 },
                            { AttributeType.CriticalRate, 900 },
                            { AttributeType.CriticalDamage, 1300 },
                            { AttributeType.UPRegeneration, 800 },
                        };
                }
            }

            Dictionary<AttributeType, int> Tactician()
            {
                switch (star)
                {
                    default: return new();
                    case 1:
                        return new()
                        {
                            { AttributeType.HpMax, 280 },
                            { AttributeType.Attack, 20 },
                            { AttributeType.Defense, 20 },
                            { AttributeType.Speed, 10 },
                            { AttributeType.DodgeRate, 150 },
                            { AttributeType.DodgeDamage, 200 },
                            { AttributeType.EffectHitRate, 300 },
                            { AttributeType.UPRegeneration, 200 },
                        };
                    case 2:
                        return new()
                        {
                            { AttributeType.HpMax, 660 },
                            { AttributeType.Attack, 50 },
                            { AttributeType.Defense, 40 },
                            { AttributeType.Speed, 45 },
                            { AttributeType.DodgeRate, 300 },
                            { AttributeType.DodgeDamage, 500 },
                            { AttributeType.EffectHitRate, 600 },
                            { AttributeType.UPRegeneration, 250 },
                        };
                    case 3:
                        return new()
                        {
                            { AttributeType.HpMax, 1020 },
                            { AttributeType.Attack, 140 },
                            { AttributeType.Defense, 80 },
                            { AttributeType.Speed, 60 },
                            { AttributeType.DodgeRate, 500 },
                            { AttributeType.DodgeDamage, 700 },
                            { AttributeType.EffectHitRate, 900 },
                            { AttributeType.UPRegeneration, 450 },
                        };
                    case 4:
                        return new()
                        {
                            { AttributeType.HpMax, 1700 },
                            { AttributeType.Attack, 200 },
                            { AttributeType.Defense, 95 },
                            { AttributeType.Speed, 135 },
                            { AttributeType.DodgeRate, 650 },
                            { AttributeType.DodgeDamage, 900 },
                            { AttributeType.EffectHitRate, 1300 },
                            { AttributeType.UPRegeneration, 600 },
                        };
                    case 5:
                        return new()
                        {
                            { AttributeType.HpMax, 2700 },
                            { AttributeType.Attack, 300 },
                            { AttributeType.Defense, 135 },
                            { AttributeType.Speed, 240 },
                            { AttributeType.DodgeRate, 800 },
                            { AttributeType.DodgeDamage, 1400 },
                            { AttributeType.EffectHitRate, 1900 },
                            { AttributeType.UPRegeneration, 650 },
                        };
                }
            }

            Dictionary<AttributeType, int> Guardian()
            {
                switch (star)
                {
                    default: return new();
                    case 1:
                        return new()
                        {
                            { AttributeType.HpMax, 450 },
                            { AttributeType.Attack, 20 },
                            { AttributeType.Defense, 65 },
                            { AttributeType.Speed, 50 },
                            { AttributeType.DodgeRate, 200 },
                            { AttributeType.DodgeDamage, 350 },
                        };
                    case 2:
                        return new()
                        {
                            { AttributeType.HpMax, 900 },
                            { AttributeType.Attack, 40 },
                            { AttributeType.Defense, 125 },
                            { AttributeType.Speed, 110 },
                            { AttributeType.DodgeRate, 400 },
                            { AttributeType.DodgeDamage, 500 },
                            { AttributeType.EffectResistRate, 300 },
                            { AttributeType.HealingReceived, 200 },
                        };
                    case 3:
                        return new()
                        {
                            { AttributeType.HpMax, 1450 },
                            { AttributeType.Attack, 110 },
                            { AttributeType.Defense, 230 },
                            { AttributeType.Speed, 135 },
                            { AttributeType.DodgeRate, 700 },
                            { AttributeType.DodgeDamage, 800 },
                            { AttributeType.EffectResistRate, 450 },
                            { AttributeType.HealingReceived, 350 },
                        };
                    case 4:
                        return new()
                        {
                            { AttributeType.HpMax, 2500 },
                            { AttributeType.Attack, 160 },
                            { AttributeType.Defense, 380 },
                            { AttributeType.Speed, 250 },
                            { AttributeType.DodgeRate, 900 },
                            { AttributeType.DodgeDamage, 1100 },
                            { AttributeType.EffectResistRate, 650 },
                            { AttributeType.HealingReceived, 450 },
                        };
                    case 5:
                        return new()
                        {
                            { AttributeType.HpMax, 3900 },
                            { AttributeType.Attack, 270 },
                            { AttributeType.Defense, 600 },
                            { AttributeType.Speed, 380 },
                            { AttributeType.DodgeRate, 1500 },
                            { AttributeType.DodgeDamage, 2000 },
                            { AttributeType.EffectResistRate, 800 },
                            { AttributeType.HealingReceived, 700 },
                        };
                }
            }

            Dictionary<AttributeType, int> Medic()
            {
                switch (star)
                {
                    default: return new();
                    case 1:
                        return new()
                        {
                            { AttributeType.HpMax, 300 },
                            { AttributeType.Attack, 20 },
                            { AttributeType.Defense, 20 },
                            { AttributeType.Speed, 70 },
                            { AttributeType.DodgeRate, 200 },
                            { AttributeType.DodgeDamage, 350 },
                            { AttributeType.OutgoingHealing, 350 },
                            { AttributeType.EffectHitRate, 350 },
                        };
                    case 2:
                        return new()
                        {
                            { AttributeType.HpMax, 700 },
                            { AttributeType.Attack, 40 },
                            { AttributeType.Defense, 50 },
                            { AttributeType.Speed, 150 },
                            { AttributeType.DodgeRate, 400 },
                            { AttributeType.DodgeDamage, 500 },
                            { AttributeType.OutgoingHealing, 500 },
                            { AttributeType.EffectHitRate, 550 },
                        };
                    case 3:
                        return new()
                        {
                            { AttributeType.HpMax, 1280 },
                            { AttributeType.Attack, 95 },
                            { AttributeType.Defense, 110 },
                            { AttributeType.Speed, 240 },
                            { AttributeType.DodgeRate, 600 },
                            { AttributeType.DodgeDamage, 750 },
                            { AttributeType.OutgoingHealing, 700 },
                            { AttributeType.EffectHitRate, 700 },
                        };
                    case 4:
                        return new()
                        {
                            { AttributeType.HpMax, 1600 },
                            { AttributeType.Attack, 150 },
                            { AttributeType.Defense, 230 },
                            { AttributeType.Speed, 280 },
                            { AttributeType.DodgeRate, 750 },
                            { AttributeType.DodgeDamage, 850 },
                            { AttributeType.OutgoingHealing, 850 },
                            { AttributeType.EffectHitRate, 850 },
                        };
                    case 5:
                        return new()
                        {
                            { AttributeType.HpMax, 2100 },
                            { AttributeType.Attack, 250 },
                            { AttributeType.Defense, 300 },
                            { AttributeType.Speed, 300 },
                            { AttributeType.DodgeRate, 900 },
                            { AttributeType.DodgeDamage, 1000 },
                            { AttributeType.OutgoingHealing, 1300 },
                            { AttributeType.EffectHitRate, 1000 },
                        };
                }
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