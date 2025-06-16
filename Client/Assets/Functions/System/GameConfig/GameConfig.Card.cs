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
                int hpRate = level <= 30 ? 80 : 120; // 2400 + 2400 = 4800
                int atkRate = level <= 30 ? 10 : 14; // 300 + 280 = 580
                int defRate = level <= 30 ? 10 : 12; // 300 + 240 = 540
                int spdRate = level <= 30 ? 2 : 3; // 60 + 60 = 120
                int critRate = level <= 30 ? 6 : 8; // 180 + 160 = 340 (~3.4%)
                int critDmg = level <= 30 ? 14 : 18; // 420 + 360 = 780 (~7.8%)

                return new()
                {
                    { AttributeType.HpMax, hpRate * level },
                    { AttributeType.Attack, atkRate * level },
                    { AttributeType.Defense, defRate * level },
                    { AttributeType.Speed, spdRate * level },
                    { AttributeType.CriticalRate, critRate * level },
                    { AttributeType.CriticalDamage, critDmg * level },
                };
            }


            Dictionary<AttributeType, int> Assassin()
            {
                int hpRate = level <= 30 ? 40 : 60; // 1200 + 1200 = 2400
                int atkRate = level <= 30 ? 12 : 16; // 360 + 320 = 680
                int defRate = level <= 30 ? 4 : 6; // 120 + 120 = 240
                int spdRate = level <= 30 ? 4 : 5; // 120 + 100 = 220
                int penRate = level <= 30 ? 8 : 10; // 240 + 200 = 440
                int penDmg = level <= 30 ? 12 : 14; // 360 + 280 = 640

                return new()
                {
                    { AttributeType.HpMax, hpRate * level },
                    { AttributeType.Attack, atkRate * level },
                    { AttributeType.Defense, defRate * level },
                    { AttributeType.Speed, spdRate * level },
                    { AttributeType.ArmorPenetrationRate, penRate * level },
                    { AttributeType.ArmorPenetrationDamage, penDmg * level },
                };
            }


            Dictionary<AttributeType, int> Demolisher()
            {
                int hpRate = level <= 30 ? 55 : 85; // 1650 + 1700 = 3350
                int atkRate = level <= 30 ? 10 : 14; // 300 + 280 = 580
                int defRate = level <= 30 ? 6 : 8; // 180 + 160 = 340
                int spdRate = level <= 30 ? 2 : 3; // 60 + 60 = 120
                int upRate = level <= 30 ? 6 : 8; // 180 + 160 = 340 (3.4%)

                return new()
                {
                    { AttributeType.HpMax, hpRate * level },
                    { AttributeType.Attack, atkRate * level },
                    { AttributeType.Defense, defRate * level },
                    { AttributeType.Speed, spdRate * level },
                    { AttributeType.UPRegeneration, upRate * level },
                };
            }


            Dictionary<AttributeType, int> Tactician()
            {
                int hpRate = level <= 30 ? 40 : 60; // 1200 + 1200 = 2400
                int atkRate = level <= 30 ? 8 : 10; // 240 + 200 = 440
                int defRate = level <= 30 ? 6 : 8; // 180 + 160 = 340
                int spdRate = level <= 30 ? 3 : 4; // 90 + 80 = 170
                int dodgeRate = level <= 30 ? 6 : 8; // 180 + 160 = 340
                int dodgeDmg = level <= 30 ? 8 : 10; // 240 + 200 = 440
                int upRate = level <= 30 ? 6 : 8; // 180 + 160 = 340

                return new()
                {
                    { AttributeType.HpMax, hpRate * level },
                    { AttributeType.Attack, atkRate * level },
                    { AttributeType.Defense, defRate * level },
                    { AttributeType.Speed, spdRate * level },
                    { AttributeType.DodgeRate, dodgeRate * level },
                    { AttributeType.DodgeDamage, dodgeDmg * level },
                    { AttributeType.UPRegeneration, upRate * level },
                };
            }


            Dictionary<AttributeType, int> Guardian()
            {
                int hpRate = level <= 30 ? 90 : 130; // 2700 + 2600 = 5300
                int atkRate = level <= 30 ? 6 : 10; // 180 + 200 = 380
                int defRate = level <= 30 ? 12 : 16; // 360 + 320 = 680
                int spdRate = level <= 30 ? 1 : 2; // 30 + 40 = 70
                int dodgeRate = level <= 30 ? 8 : 10; // 240 + 200 = 440 (~4.4%)
                int dodgeDmg = level <= 30 ? 12 : 14; // 360 + 280 = 640 (~6.4%)
                int resistRate = level <= 30 ? 8 : 10; // 240 + 200 = 440 (~4.4%)
                int healingRec = level <= 30 ? 8 : 10; // 240 + 200 = 440 (~4.4%)

                return new()
                {
                    { AttributeType.HpMax, hpRate * level },
                    { AttributeType.Attack, atkRate * level },
                    { AttributeType.Defense, defRate * level },
                    { AttributeType.Speed, spdRate * level },
                    { AttributeType.DodgeRate, dodgeRate * level },
                    { AttributeType.DodgeDamage, dodgeDmg * level },
                    { AttributeType.EffectResistRate, resistRate * level },
                    { AttributeType.HealingReceived, healingRec * level },
                };
            }


            Dictionary<AttributeType, int> Medic()
            {
                int hpRate = level <= 30 ? 70 : 110; // 2100 + 2200 = 4300
                int atkRate = level <= 30 ? 6 : 9; // 180 + 180 = 360
                int defRate = level <= 30 ? 10 : 12; // 300 + 240 = 540
                int spdRate = level <= 30 ? 2 : 3; // 60 + 60 = 120
                int outHeal = level <= 30 ? 8 : 10; // 240 + 200 = 440 (~4.4%)
                int effHit = level <= 30 ? 8 : 10; // 240 + 200 = 440
                int upRate = level <= 30 ? 6 : 8; // 180 + 160 = 340

                return new()
                {
                    { AttributeType.HpMax, hpRate * level },
                    { AttributeType.Attack, atkRate * level },
                    { AttributeType.Defense, defRate * level },
                    { AttributeType.Speed, spdRate * level },
                    { AttributeType.OutgoingHealing, outHeal * level },
                    { AttributeType.EffectHitRate, effHit * level },
                    { AttributeType.UPRegeneration, upRate * level },
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
        public Dictionary<AttributeType, int> CardStarAttributeBonus(ClassType classType, ElementType elementType,
            byte star)
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
                switch (elementType)
                {
                    case ElementType.Metal:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 400 },
                                    { AttributeType.Attack, 40 },
                                    { AttributeType.Defense, 50 },
                                    { AttributeType.Speed, 60 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 750 },
                                    { AttributeType.Attack, 100 },
                                    { AttributeType.Defense, 120 },
                                    { AttributeType.Speed, 140 },
                                    { AttributeType.ToughnessHitRate, 500 },
                                    { AttributeType.CriticalRate, 300 },
                                    { AttributeType.CriticalDamage, 500 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1100 },
                                    { AttributeType.Attack, 180 },
                                    { AttributeType.Defense, 180 },
                                    { AttributeType.Speed, 210 },
                                    { AttributeType.EffectResistRate, 300 },
                                    { AttributeType.ToughnessHitRate, 750 },
                                    { AttributeType.CriticalRate, 500 },
                                    { AttributeType.CriticalDamage, 750 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1800 },
                                    { AttributeType.Attack, 250 },
                                    { AttributeType.Defense, 260 },
                                    { AttributeType.Speed, 320 },
                                    { AttributeType.EffectResistRate, 500 },
                                    { AttributeType.ToughnessHitRate, 950 },
                                    { AttributeType.CriticalRate, 700 },
                                    { AttributeType.CriticalDamage, 1000 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3200 },
                                    { AttributeType.Attack, 400 },
                                    { AttributeType.Defense, 420 },
                                    { AttributeType.Speed, 500 },
                                    { AttributeType.EffectResistRate, 700 },
                                    { AttributeType.ToughnessHitRate, 1200 },
                                    { AttributeType.CriticalRate, 1000 },
                                    { AttributeType.CriticalDamage, 1400 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                        }
                    case ElementType.Fire:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 320 },
                                    { AttributeType.Attack, 60 },
                                    { AttributeType.CriticalRate, 200 },
                                    { AttributeType.CriticalDamage, 300 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 600 },
                                    { AttributeType.Attack, 120 },
                                    { AttributeType.CriticalRate, 400 },
                                    { AttributeType.CriticalDamage, 600 },
                                    { AttributeType.Speed, 120 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 900 },
                                    { AttributeType.Attack, 200 },
                                    { AttributeType.CriticalRate, 650 },
                                    { AttributeType.CriticalDamage, 850 },
                                    { AttributeType.Speed, 180 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1500 },
                                    { AttributeType.Attack, 300 },
                                    { AttributeType.CriticalRate, 950 },
                                    { AttributeType.CriticalDamage, 1200 },
                                    { AttributeType.Speed, 250 },
                                    { AttributeType.ToughnessHitRate, 700 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2700 },
                                    { AttributeType.Attack, 480 },
                                    { AttributeType.CriticalRate, 1400 },
                                    { AttributeType.CriticalDamage, 1800 },
                                    { AttributeType.Speed, 350 },
                                    { AttributeType.ToughnessHitRate, 1000 },
                                    { AttributeType.UPRegeneration, 400 },
                                };
                        }
                    case ElementType.Wood:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 300 },
                                    { AttributeType.Attack, 35 },
                                    { AttributeType.Speed, 80 },
                                    { AttributeType.DodgeRate, 200 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 600 },
                                    { AttributeType.Attack, 90 },
                                    { AttributeType.Speed, 160 },
                                    { AttributeType.DodgeRate, 350 },
                                    { AttributeType.CriticalRate, 300 },
                                    { AttributeType.ToughnessHitRate, 400 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 950 },
                                    { AttributeType.Attack, 150 },
                                    { AttributeType.Speed, 220 },
                                    { AttributeType.DodgeRate, 500 },
                                    { AttributeType.CriticalRate, 450 },
                                    { AttributeType.ToughnessHitRate, 600 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1600 },
                                    { AttributeType.Attack, 220 },
                                    { AttributeType.Speed, 300 },
                                    { AttributeType.DodgeRate, 650 },
                                    { AttributeType.CriticalRate, 650 },
                                    { AttributeType.ToughnessHitRate, 850 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2700 },
                                    { AttributeType.Attack, 340 },
                                    { AttributeType.Speed, 420 },
                                    { AttributeType.DodgeRate, 1000 },
                                    { AttributeType.CriticalRate, 900 },
                                    { AttributeType.CriticalDamage, 1200 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                };
                        }
                    case ElementType.Water:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 380 },
                                    { AttributeType.Attack, 30 },
                                    { AttributeType.Defense, 50 },
                                    { AttributeType.EffectHitRate, 200 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 700 },
                                    { AttributeType.Attack, 85 },
                                    { AttributeType.Defense, 100 },
                                    { AttributeType.EffectHitRate, 400 },
                                    { AttributeType.ToughnessHitRate, 400 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1050 },
                                    { AttributeType.Attack, 140 },
                                    { AttributeType.Defense, 160 },
                                    { AttributeType.EffectHitRate, 600 },
                                    { AttributeType.ToughnessHitRate, 650 },
                                    { AttributeType.HealingReceived, 250 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1700 },
                                    { AttributeType.Attack, 210 },
                                    { AttributeType.Defense, 250 },
                                    { AttributeType.EffectHitRate, 800 },
                                    { AttributeType.ToughnessHitRate, 850 },
                                    { AttributeType.HealingReceived, 400 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2900 },
                                    { AttributeType.Attack, 320 },
                                    { AttributeType.Defense, 400 },
                                    { AttributeType.EffectHitRate, 1000 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                    { AttributeType.HealingReceived, 600 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                        }
                    case ElementType.Earth:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 500 },
                                    { AttributeType.Attack, 30 },
                                    { AttributeType.Defense, 80 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 900 },
                                    { AttributeType.Attack, 70 },
                                    { AttributeType.Defense, 160 },
                                    { AttributeType.EffectResistRate, 300 },
                                    { AttributeType.ToughnessHitRate, 450 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1350 },
                                    { AttributeType.Attack, 110 },
                                    { AttributeType.Defense, 240 },
                                    { AttributeType.EffectResistRate, 500 },
                                    { AttributeType.ToughnessHitRate, 700 },
                                    { AttributeType.HealingReceived, 250 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 2000 },
                                    { AttributeType.Attack, 180 },
                                    { AttributeType.Defense, 350 },
                                    { AttributeType.EffectResistRate, 700 },
                                    { AttributeType.ToughnessHitRate, 950 },
                                    { AttributeType.HealingReceived, 400 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3400 },
                                    { AttributeType.Attack, 300 },
                                    { AttributeType.Defense, 500 },
                                    { AttributeType.EffectResistRate, 1000 },
                                    { AttributeType.ToughnessHitRate, 1200 },
                                    { AttributeType.HealingReceived, 600 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                        }
                    default: return new();
                }
            }


            Dictionary<AttributeType, int> Assassin()
            {
                switch (elementType)
                {
                    case ElementType.Metal:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 250 },
                                    { AttributeType.Attack, 70 },
                                    { AttributeType.Speed, 80 },
                                    { AttributeType.ArmorPenetrationRate, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 480 },
                                    { AttributeType.Attack, 140 },
                                    { AttributeType.Speed, 160 },
                                    { AttributeType.ArmorPenetrationRate, 500 },
                                    { AttributeType.ArmorPenetrationDamage, 500 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 750 },
                                    { AttributeType.Attack, 220 },
                                    { AttributeType.Speed, 230 },
                                    { AttributeType.ArmorPenetrationRate, 750 },
                                    { AttributeType.ArmorPenetrationDamage, 800 },
                                    { AttributeType.ToughnessHitRate, 600 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1200 },
                                    { AttributeType.Attack, 320 },
                                    { AttributeType.Speed, 320 },
                                    { AttributeType.ArmorPenetrationRate, 1000 },
                                    { AttributeType.ArmorPenetrationDamage, 1100 },
                                    { AttributeType.ToughnessHitRate, 800 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2000 },
                                    { AttributeType.Attack, 520 },
                                    { AttributeType.Speed, 450 },
                                    { AttributeType.ArmorPenetrationRate, 1400 },
                                    { AttributeType.ArmorPenetrationDamage, 1600 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                    { AttributeType.CriticalRate, 800 },
                                };
                        }
                    case ElementType.Fire:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 200 },
                                    { AttributeType.Attack, 80 },
                                    { AttributeType.CriticalRate, 300 },
                                    { AttributeType.Speed, 90 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 420 },
                                    { AttributeType.Attack, 160 },
                                    { AttributeType.CriticalRate, 600 },
                                    { AttributeType.Speed, 160 },
                                    { AttributeType.CriticalDamage, 500 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 700 },
                                    { AttributeType.Attack, 260 },
                                    { AttributeType.CriticalRate, 850 },
                                    { AttributeType.Speed, 240 },
                                    { AttributeType.CriticalDamage, 850 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1100 },
                                    { AttributeType.Attack, 380 },
                                    { AttributeType.CriticalRate, 1100 },
                                    { AttributeType.Speed, 340 },
                                    { AttributeType.CriticalDamage, 1200 },
                                    { AttributeType.ToughnessHitRate, 600 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 1800 },
                                    { AttributeType.Attack, 600 },
                                    { AttributeType.CriticalRate, 1500 },
                                    { AttributeType.Speed, 500 },
                                    { AttributeType.CriticalDamage, 1600 },
                                    { AttributeType.ToughnessHitRate, 1000 },
                                    { AttributeType.UPRegeneration, 400 },
                                };
                        }
                    case ElementType.Wood:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 220 },
                                    { AttributeType.Attack, 60 },
                                    { AttributeType.Speed, 100 },
                                    { AttributeType.DodgeRate, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 460 },
                                    { AttributeType.Attack, 120 },
                                    { AttributeType.Speed, 180 },
                                    { AttributeType.DodgeRate, 500 },
                                    { AttributeType.EffectHitRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 720 },
                                    { AttributeType.Attack, 190 },
                                    { AttributeType.Speed, 250 },
                                    { AttributeType.DodgeRate, 700 },
                                    { AttributeType.EffectHitRate, 500 },
                                    { AttributeType.ToughnessHitRate, 600 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1200 },
                                    { AttributeType.Attack, 300 },
                                    { AttributeType.Speed, 330 },
                                    { AttributeType.DodgeRate, 900 },
                                    { AttributeType.EffectHitRate, 700 },
                                    { AttributeType.ToughnessHitRate, 800 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2000 },
                                    { AttributeType.Attack, 500 },
                                    { AttributeType.Speed, 500 },
                                    { AttributeType.DodgeRate, 1200 },
                                    { AttributeType.EffectHitRate, 1000 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                    { AttributeType.CriticalRate, 800 },
                                };
                        }
                    case ElementType.Water:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 240 },
                                    { AttributeType.Attack, 55 },
                                    { AttributeType.EffectHitRate, 300 },
                                    { AttributeType.Speed, 80 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 500 },
                                    { AttributeType.Attack, 120 },
                                    { AttributeType.EffectHitRate, 600 },
                                    { AttributeType.Speed, 150 },
                                    { AttributeType.ToughnessHitRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 800 },
                                    { AttributeType.Attack, 190 },
                                    { AttributeType.EffectHitRate, 900 },
                                    { AttributeType.Speed, 220 },
                                    { AttributeType.ToughnessHitRate, 500 },
                                    { AttributeType.CriticalRate, 350 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1300 },
                                    { AttributeType.Attack, 270 },
                                    { AttributeType.EffectHitRate, 1200 },
                                    { AttributeType.Speed, 300 },
                                    { AttributeType.ToughnessHitRate, 700 },
                                    { AttributeType.CriticalRate, 600 },
                                    { AttributeType.UPRegeneration, 300 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2200 },
                                    { AttributeType.Attack, 450 },
                                    { AttributeType.EffectHitRate, 1600 },
                                    { AttributeType.Speed, 440 },
                                    { AttributeType.ToughnessHitRate, 1000 },
                                    { AttributeType.CriticalRate, 900 },
                                    { AttributeType.CriticalDamage, 1200 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                        }
                    case ElementType.Earth:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 300 },
                                    { AttributeType.Attack, 50 },
                                    { AttributeType.Defense, 50 },
                                    { AttributeType.ToughnessHitRate, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 600 },
                                    { AttributeType.Attack, 110 },
                                    { AttributeType.Defense, 100 },
                                    { AttributeType.ToughnessHitRate, 500 },
                                    { AttributeType.EffectResistRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 950 },
                                    { AttributeType.Attack, 170 },
                                    { AttributeType.Defense, 160 },
                                    { AttributeType.ToughnessHitRate, 750 },
                                    { AttributeType.EffectResistRate, 500 },
                                    { AttributeType.CriticalDamage, 700 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1500 },
                                    { AttributeType.Attack, 240 },
                                    { AttributeType.Defense, 240 },
                                    { AttributeType.ToughnessHitRate, 1000 },
                                    { AttributeType.EffectResistRate, 700 },
                                    { AttributeType.CriticalDamage, 1000 },
                                    { AttributeType.UPRegeneration, 250 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2400 },
                                    { AttributeType.Attack, 400 },
                                    { AttributeType.Defense, 360 },
                                    { AttributeType.ToughnessHitRate, 1300 },
                                    { AttributeType.EffectResistRate, 900 },
                                    { AttributeType.CriticalDamage, 1400 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                        }
                    default: return new();
                }
            }

            Dictionary<AttributeType, int> Demolisher()
            {
                switch (elementType)
                {
                    case ElementType.Metal:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 380 }, { AttributeType.Attack, 60 },
                                    { AttributeType.Defense, 80 }, { AttributeType.UPRegeneration, 200 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 720 }, { AttributeType.Attack, 130 },
                                    { AttributeType.Defense, 150 }, { AttributeType.UPRegeneration, 300 },
                                    { AttributeType.ArmorPenetrationDamage, 500 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1100 }, { AttributeType.Attack, 190 },
                                    { AttributeType.Defense, 240 }, { AttributeType.UPRegeneration, 450 },
                                    { AttributeType.ArmorPenetrationRate, 400 },
                                    { AttributeType.ToughnessHitRate, 500 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1800 }, { AttributeType.Attack, 270 },
                                    { AttributeType.Defense, 360 }, { AttributeType.UPRegeneration, 600 },
                                    { AttributeType.ArmorPenetrationRate, 600 },
                                    { AttributeType.ToughnessHitRate, 800 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3000 }, { AttributeType.Attack, 400 },
                                    { AttributeType.Defense, 500 }, { AttributeType.UPRegeneration, 800 },
                                    { AttributeType.ArmorPenetrationRate, 900 },
                                    { AttributeType.ArmorPenetrationDamage, 1200 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                };
                        }
                    case ElementType.Fire:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 300 }, { AttributeType.Attack, 90 },
                                    { AttributeType.CriticalRate, 300 }, { AttributeType.CriticalDamage, 500 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 580 }, { AttributeType.Attack, 160 },
                                    { AttributeType.CriticalRate, 550 }, { AttributeType.CriticalDamage, 750 },
                                    { AttributeType.UPRegeneration, 200 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 900 }, { AttributeType.Attack, 230 },
                                    { AttributeType.CriticalRate, 800 }, { AttributeType.CriticalDamage, 1050 },
                                    { AttributeType.UPRegeneration, 400 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1500 }, { AttributeType.Attack, 320 },
                                    { AttributeType.CriticalRate, 1100 }, { AttributeType.CriticalDamage, 1400 },
                                    { AttributeType.UPRegeneration, 600 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2600 }, { AttributeType.Attack, 480 },
                                    { AttributeType.CriticalRate, 1600 }, { AttributeType.CriticalDamage, 2000 },
                                    { AttributeType.UPRegeneration, 800 }, { AttributeType.ToughnessHitRate, 1000 },
                                };
                        }
                    case ElementType.Wood:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 320 }, { AttributeType.Attack, 70 },
                                    { AttributeType.EffectHitRate, 300 }, { AttributeType.Speed, 60 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 640 }, { AttributeType.Attack, 130 },
                                    { AttributeType.EffectHitRate, 600 }, { AttributeType.Speed, 110 },
                                    { AttributeType.UPRegeneration, 200 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1000 }, { AttributeType.Attack, 190 },
                                    { AttributeType.EffectHitRate, 900 }, { AttributeType.Speed, 180 },
                                    { AttributeType.UPRegeneration, 400 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1600 }, { AttributeType.Attack, 260 },
                                    { AttributeType.EffectHitRate, 1200 }, { AttributeType.Speed, 240 },
                                    { AttributeType.UPRegeneration, 600 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2700 }, { AttributeType.Attack, 400 },
                                    { AttributeType.EffectHitRate, 1600 }, { AttributeType.Speed, 330 },
                                    { AttributeType.UPRegeneration, 800 }, { AttributeType.ToughnessHitRate, 900 },
                                };
                        }
                    case ElementType.Water:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 360 }, { AttributeType.Attack, 60 },
                                    { AttributeType.EffectResistRate, 300 }, { AttributeType.UPRegeneration, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 700 }, { AttributeType.Attack, 130 },
                                    { AttributeType.EffectResistRate, 500 }, { AttributeType.UPRegeneration, 400 },
                                    { AttributeType.ToughnessHitRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1100 }, { AttributeType.Attack, 190 },
                                    { AttributeType.EffectResistRate, 700 }, { AttributeType.UPRegeneration, 550 },
                                    { AttributeType.ToughnessHitRate, 500 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1700 }, { AttributeType.Attack, 270 },
                                    { AttributeType.EffectResistRate, 900 }, { AttributeType.UPRegeneration, 700 },
                                    { AttributeType.ToughnessHitRate, 800 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2900 }, { AttributeType.Attack, 420 },
                                    { AttributeType.EffectResistRate, 1200 }, { AttributeType.UPRegeneration, 900 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                };
                        }
                    case ElementType.Earth:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 450 }, { AttributeType.Attack, 50 },
                                    { AttributeType.Defense, 100 }, { AttributeType.ToughnessHitRate, 300 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 850 }, { AttributeType.Attack, 110 },
                                    { AttributeType.Defense, 180 }, { AttributeType.ToughnessHitRate, 500 },
                                    { AttributeType.EffectResistRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1350 }, { AttributeType.Attack, 180 },
                                    { AttributeType.Defense, 270 }, { AttributeType.ToughnessHitRate, 700 },
                                    { AttributeType.EffectResistRate, 500 }, { AttributeType.UPRegeneration, 300 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 2200 }, { AttributeType.Attack, 260 },
                                    { AttributeType.Defense, 400 }, { AttributeType.ToughnessHitRate, 900 },
                                    { AttributeType.EffectResistRate, 700 }, { AttributeType.UPRegeneration, 500 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3500 }, { AttributeType.Attack, 400 },
                                    { AttributeType.Defense, 600 }, { AttributeType.ToughnessHitRate, 1200 },
                                    { AttributeType.EffectResistRate, 1000 }, { AttributeType.UPRegeneration, 800 },
                                };
                        }
                    default: return new();
                }
            }


            Dictionary<AttributeType, int> Tactician()
            {
                switch (elementType)
                {
                    case ElementType.Metal:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 250 }, { AttributeType.Attack, 30 },
                                    { AttributeType.Defense, 50 }, { AttributeType.EffectHitRate, 300 }
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 520 }, { AttributeType.Attack, 80 },
                                    { AttributeType.Defense, 90 }, { AttributeType.EffectHitRate, 600 },
                                    { AttributeType.UPRegeneration, 200 }
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 880 }, { AttributeType.Attack, 130 },
                                    { AttributeType.Defense, 140 }, { AttributeType.EffectHitRate, 900 },
                                    { AttributeType.UPRegeneration, 350 }, { AttributeType.ToughnessHitRate, 400 }
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1450 }, { AttributeType.Attack, 190 },
                                    { AttributeType.Defense, 200 }, { AttributeType.EffectHitRate, 1300 },
                                    { AttributeType.UPRegeneration, 500 }, { AttributeType.ToughnessHitRate, 600 }
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2300 }, { AttributeType.Attack, 280 },
                                    { AttributeType.Defense, 300 }, { AttributeType.EffectHitRate, 1800 },
                                    { AttributeType.UPRegeneration, 700 }, { AttributeType.ToughnessHitRate, 900 }
                                };
                        }
                    case ElementType.Fire:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 220 }, { AttributeType.Attack, 60 },
                                    { AttributeType.Speed, 80 }, { AttributeType.DodgeRate, 200 }
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 460 }, { AttributeType.Attack, 120 },
                                    { AttributeType.Speed, 140 }, { AttributeType.DodgeRate, 400 },
                                    { AttributeType.UPRegeneration, 200 }
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 750 }, { AttributeType.Attack, 180 },
                                    { AttributeType.Speed, 200 }, { AttributeType.DodgeRate, 600 },
                                    { AttributeType.UPRegeneration, 350 }, { AttributeType.CriticalRate, 400 }
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1200 }, { AttributeType.Attack, 270 },
                                    { AttributeType.Speed, 280 }, { AttributeType.DodgeRate, 800 },
                                    { AttributeType.UPRegeneration, 500 }, { AttributeType.CriticalRate, 600 }
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2000 }, { AttributeType.Attack, 400 },
                                    { AttributeType.Speed, 400 }, { AttributeType.DodgeRate, 1100 },
                                    { AttributeType.UPRegeneration, 700 }, { AttributeType.CriticalRate, 900 }
                                };
                        }
                    case ElementType.Wood:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 280 }, { AttributeType.Attack, 40 },
                                    { AttributeType.Defense, 30 }, { AttributeType.Speed, 60 }
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 580 }, { AttributeType.Attack, 90 },
                                    { AttributeType.Defense, 70 }, { AttributeType.Speed, 120 },
                                    { AttributeType.EffectHitRate, 300 }
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 950 }, { AttributeType.Attack, 140 },
                                    { AttributeType.Defense, 120 }, { AttributeType.Speed, 180 },
                                    { AttributeType.EffectHitRate, 500 }, { AttributeType.DodgeRate, 300 }
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1550 }, { AttributeType.Attack, 220 },
                                    { AttributeType.Defense, 180 }, { AttributeType.Speed, 250 },
                                    { AttributeType.EffectHitRate, 800 }, { AttributeType.DodgeRate, 600 }
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2600 }, { AttributeType.Attack, 320 },
                                    { AttributeType.Defense, 270 }, { AttributeType.Speed, 370 },
                                    { AttributeType.EffectHitRate, 1100 }, { AttributeType.DodgeRate, 900 }
                                };
                        }
                    case ElementType.Water:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 310 }, { AttributeType.Attack, 35 },
                                    { AttributeType.Defense, 60 }, { AttributeType.UPRegeneration, 250 }
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 640 }, { AttributeType.Attack, 80 },
                                    { AttributeType.Defense, 120 }, { AttributeType.UPRegeneration, 400 },
                                    { AttributeType.EffectHitRate, 300 }
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1000 }, { AttributeType.Attack, 130 },
                                    { AttributeType.Defense, 190 }, { AttributeType.UPRegeneration, 600 },
                                    { AttributeType.EffectHitRate, 500 }, { AttributeType.HealingReceived, 200 }
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1650 }, { AttributeType.Attack, 200 },
                                    { AttributeType.Defense, 270 }, { AttributeType.UPRegeneration, 800 },
                                    { AttributeType.EffectHitRate, 800 }, { AttributeType.HealingReceived, 400 }
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2700 }, { AttributeType.Attack, 300 },
                                    { AttributeType.Defense, 400 }, { AttributeType.UPRegeneration, 1000 },
                                    { AttributeType.EffectHitRate, 1200 }, { AttributeType.HealingReceived, 600 }
                                };
                        }
                    case ElementType.Earth:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 400 }, { AttributeType.Attack, 20 },
                                    { AttributeType.Defense, 90 }, { AttributeType.EffectResistRate, 300 }
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 800 }, { AttributeType.Attack, 60 },
                                    { AttributeType.Defense, 160 }, { AttributeType.EffectResistRate, 500 },
                                    { AttributeType.UPRegeneration, 200 }
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1250 }, { AttributeType.Attack, 110 },
                                    { AttributeType.Defense, 240 }, { AttributeType.EffectResistRate, 700 },
                                    { AttributeType.UPRegeneration, 400 }, { AttributeType.ToughnessHitRate, 500 }
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1900 }, { AttributeType.Attack, 170 },
                                    { AttributeType.Defense, 330 }, { AttributeType.EffectResistRate, 950 },
                                    { AttributeType.UPRegeneration, 600 }, { AttributeType.ToughnessHitRate, 800 }
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3000 }, { AttributeType.Attack, 250 },
                                    { AttributeType.Defense, 500 }, { AttributeType.EffectResistRate, 1300 },
                                    { AttributeType.UPRegeneration, 800 }, { AttributeType.ToughnessHitRate, 1100 }
                                };
                        }
                    default: return new();
                }
            }


            Dictionary<AttributeType, int> Guardian()
            {
                switch (elementType)
                {
                    case ElementType.Metal:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 450 }, { AttributeType.Defense, 90 },
                                    { AttributeType.EffectResistRate, 200 }, { AttributeType.HealingReceived, 150 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 850 }, { AttributeType.Defense, 180 },
                                    { AttributeType.EffectResistRate, 400 }, { AttributeType.HealingReceived, 250 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1350 }, { AttributeType.Defense, 280 },
                                    { AttributeType.EffectResistRate, 600 }, { AttributeType.HealingReceived, 400 },
                                    { AttributeType.ToughnessHitRate, 400 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 2100 }, { AttributeType.Defense, 400 },
                                    { AttributeType.EffectResistRate, 800 }, { AttributeType.HealingReceived, 600 },
                                    { AttributeType.ToughnessHitRate, 700 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3400 }, { AttributeType.Defense, 600 },
                                    { AttributeType.EffectResistRate, 1000 }, { AttributeType.HealingReceived, 900 },
                                    { AttributeType.ToughnessHitRate, 1100 },
                                };
                        }
                    case ElementType.Fire:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 400 }, { AttributeType.Defense, 60 },
                                    { AttributeType.DodgeDamage, 200 }, { AttributeType.CriticalDamage, 300 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 800 }, { AttributeType.Defense, 130 },
                                    { AttributeType.DodgeDamage, 400 }, { AttributeType.CriticalDamage, 500 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1300 }, { AttributeType.Defense, 210 },
                                    { AttributeType.DodgeDamage, 600 }, { AttributeType.CriticalDamage, 800 },
                                    { AttributeType.ToughnessHitRate, 400 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 2000 }, { AttributeType.Defense, 300 },
                                    { AttributeType.DodgeDamage, 800 }, { AttributeType.CriticalDamage, 1000 },
                                    { AttributeType.ToughnessHitRate, 700 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3300 }, { AttributeType.Defense, 500 },
                                    { AttributeType.DodgeDamage, 1100 }, { AttributeType.CriticalDamage, 1300 },
                                    { AttributeType.ToughnessHitRate, 1000 },
                                };
                        }
                    case ElementType.Wood:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 350 }, { AttributeType.Defense, 50 },
                                    { AttributeType.Speed, 90 }, { AttributeType.DodgeRate, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 700 }, { AttributeType.Defense, 100 },
                                    { AttributeType.Speed, 150 }, { AttributeType.DodgeRate, 450 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1150 }, { AttributeType.Defense, 160 },
                                    { AttributeType.Speed, 220 }, { AttributeType.DodgeRate, 700 },
                                    { AttributeType.HealingReceived, 200 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1900 }, { AttributeType.Defense, 250 },
                                    { AttributeType.Speed, 300 }, { AttributeType.DodgeRate, 900 },
                                    { AttributeType.HealingReceived, 350 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3100 }, { AttributeType.Defense, 400 },
                                    { AttributeType.Speed, 420 }, { AttributeType.DodgeRate, 1200 },
                                    { AttributeType.HealingReceived, 550 },
                                };
                        }
                    case ElementType.Water:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 400 }, { AttributeType.Defense, 70 },
                                    { AttributeType.HealingReceived, 200 }, { AttributeType.EffectResistRate, 200 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 780 }, { AttributeType.Defense, 140 },
                                    { AttributeType.HealingReceived, 400 }, { AttributeType.EffectResistRate, 400 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1300 }, { AttributeType.Defense, 220 },
                                    { AttributeType.HealingReceived, 600 }, { AttributeType.EffectResistRate, 600 },
                                    { AttributeType.UPRegeneration, 250 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 2000 }, { AttributeType.Defense, 320 },
                                    { AttributeType.HealingReceived, 850 }, { AttributeType.EffectResistRate, 850 },
                                    { AttributeType.UPRegeneration, 400 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3300 }, { AttributeType.Defense, 500 },
                                    { AttributeType.HealingReceived, 1200 }, { AttributeType.EffectResistRate, 1100 },
                                    { AttributeType.UPRegeneration, 600 },
                                };
                        }
                    case ElementType.Earth:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 500 }, { AttributeType.Defense, 100 },
                                    { AttributeType.ToughnessHitRate, 300 }, { AttributeType.EffectResistRate, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 950 }, { AttributeType.Defense, 200 },
                                    { AttributeType.ToughnessHitRate, 600 }, { AttributeType.EffectResistRate, 500 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1500 }, { AttributeType.Defense, 300 },
                                    { AttributeType.ToughnessHitRate, 900 }, { AttributeType.EffectResistRate, 700 },
                                    { AttributeType.HealingReceived, 300 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 2300 }, { AttributeType.Defense, 450 },
                                    { AttributeType.ToughnessHitRate, 1200 }, { AttributeType.EffectResistRate, 900 },
                                    { AttributeType.HealingReceived, 500 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3600 }, { AttributeType.Defense, 650 },
                                    { AttributeType.ToughnessHitRate, 1500 }, { AttributeType.EffectResistRate, 1200 },
                                    { AttributeType.HealingReceived, 800 },
                                };
                        }
                    default: return new();
                }
            }

            Dictionary<AttributeType, int> Medic()
            {
                switch (elementType)
                {
                    case ElementType.Metal:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 300 }, { AttributeType.Defense, 60 },
                                    { AttributeType.OutgoingHealing, 300 }, { AttributeType.EffectHitRate, 200 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 600 }, { AttributeType.Defense, 120 },
                                    { AttributeType.OutgoingHealing, 500 }, { AttributeType.EffectHitRate, 400 },
                                    { AttributeType.UPRegeneration, 200 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 950 }, { AttributeType.Defense, 190 },
                                    { AttributeType.OutgoingHealing, 700 }, { AttributeType.EffectHitRate, 650 },
                                    { AttributeType.UPRegeneration, 350 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1500 }, { AttributeType.Defense, 280 },
                                    { AttributeType.OutgoingHealing, 950 }, { AttributeType.EffectHitRate, 900 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2300 }, { AttributeType.Defense, 400 },
                                    { AttributeType.OutgoingHealing, 1300 }, { AttributeType.EffectHitRate, 1200 },
                                    { AttributeType.UPRegeneration, 700 },
                                };
                        }
                    case ElementType.Fire:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 260 }, { AttributeType.Attack, 40 },
                                    { AttributeType.CriticalRate, 200 }, { AttributeType.OutgoingHealing, 200 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 520 }, { AttributeType.Attack, 80 },
                                    { AttributeType.CriticalRate, 400 }, { AttributeType.OutgoingHealing, 400 },
                                    { AttributeType.EffectHitRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 850 }, { AttributeType.Attack, 140 },
                                    { AttributeType.CriticalRate, 600 }, { AttributeType.OutgoingHealing, 650 },
                                    { AttributeType.EffectHitRate, 500 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1350 }, { AttributeType.Attack, 220 },
                                    { AttributeType.CriticalRate, 850 }, { AttributeType.OutgoingHealing, 850 },
                                    { AttributeType.EffectHitRate, 700 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2100 }, { AttributeType.Attack, 350 },
                                    { AttributeType.CriticalRate, 1100 }, { AttributeType.OutgoingHealing, 1100 },
                                    { AttributeType.EffectHitRate, 950 },
                                };
                        }
                    case ElementType.Wood:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 280 }, { AttributeType.Speed, 80 },
                                    { AttributeType.OutgoingHealing, 300 }, { AttributeType.EffectHitRate, 250 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 570 }, { AttributeType.Speed, 150 },
                                    { AttributeType.OutgoingHealing, 500 }, { AttributeType.EffectHitRate, 500 },
                                    { AttributeType.UPRegeneration, 200 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 940 }, { AttributeType.Speed, 230 },
                                    { AttributeType.OutgoingHealing, 750 }, { AttributeType.EffectHitRate, 700 },
                                    { AttributeType.UPRegeneration, 350 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1500 }, { AttributeType.Speed, 320 },
                                    { AttributeType.OutgoingHealing, 1000 }, { AttributeType.EffectHitRate, 950 },
                                    { AttributeType.UPRegeneration, 500 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2400 }, { AttributeType.Speed, 450 },
                                    { AttributeType.OutgoingHealing, 1300 }, { AttributeType.EffectHitRate, 1300 },
                                    { AttributeType.UPRegeneration, 700 },
                                };
                        }
                    case ElementType.Water:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 320 }, { AttributeType.OutgoingHealing, 400 },
                                    { AttributeType.HealingReceived, 200 }, { AttributeType.UPRegeneration, 150 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 650 }, { AttributeType.OutgoingHealing, 650 },
                                    { AttributeType.HealingReceived, 400 }, { AttributeType.UPRegeneration, 300 },
                                    { AttributeType.EffectResistRate, 200 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1100 }, { AttributeType.OutgoingHealing, 900 },
                                    { AttributeType.HealingReceived, 600 }, { AttributeType.UPRegeneration, 500 },
                                    { AttributeType.EffectResistRate, 400 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1700 }, { AttributeType.OutgoingHealing, 1200 },
                                    { AttributeType.HealingReceived, 800 }, { AttributeType.UPRegeneration, 700 },
                                    { AttributeType.EffectResistRate, 600 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 2600 }, { AttributeType.OutgoingHealing, 1500 },
                                    { AttributeType.HealingReceived, 1000 }, { AttributeType.UPRegeneration, 900 },
                                    { AttributeType.EffectResistRate, 900 },
                                };
                        }
                    case ElementType.Earth:
                        switch (star)
                        {
                            default: return new();
                            case 1:
                                return new()
                                {
                                    { AttributeType.HpMax, 360 }, { AttributeType.Defense, 80 },
                                    { AttributeType.OutgoingHealing, 300 }, { AttributeType.HealingReceived, 150 },
                                };
                            case 2:
                                return new()
                                {
                                    { AttributeType.HpMax, 750 }, { AttributeType.Defense, 150 },
                                    { AttributeType.OutgoingHealing, 550 }, { AttributeType.HealingReceived, 300 },
                                    { AttributeType.ToughnessHitRate, 300 },
                                };
                            case 3:
                                return new()
                                {
                                    { AttributeType.HpMax, 1200 }, { AttributeType.Defense, 230 },
                                    { AttributeType.OutgoingHealing, 800 }, { AttributeType.HealingReceived, 500 },
                                    { AttributeType.ToughnessHitRate, 500 },
                                };
                            case 4:
                                return new()
                                {
                                    { AttributeType.HpMax, 1900 }, { AttributeType.Defense, 330 },
                                    { AttributeType.OutgoingHealing, 1100 }, { AttributeType.HealingReceived, 700 },
                                    { AttributeType.ToughnessHitRate, 700 },
                                };
                            case 5:
                                return new()
                                {
                                    { AttributeType.HpMax, 3000 }, { AttributeType.Defense, 480 },
                                    { AttributeType.OutgoingHealing, 1500 }, { AttributeType.HealingReceived, 1000 },
                                    { AttributeType.ToughnessHitRate, 1000 },
                                };
                        }
                    default: return new();
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