using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using UnityEngine;
using UnityEngine.Serialization;
using World.TheCard;
using Random = UnityEngine.Random;

namespace World.TheCard
{
    public partial class CardBattle : MonoBehaviour
    {
        [SerializeField] private CardVital _vital;
        public CardVital Vital => _vital;

        [SerializeField] CardEffect _effect;

        //----------------------Data-------------------------\\
        [FormerlySerializedAs("_factionIndexInBoard")] [SerializeField]
        int factionIndex;

        [FormerlySerializedAs("_memberIndexInBoard")] [SerializeField]
        int memberIndex;

        [SerializeField] private ElementType _elementType;
        [SerializeField] private SerializedDictionary<AttributeType, int> _attributes;

        /// <summary>
        /// điểm hành động, thứ tự trong round xếp theo AP cao đến thấp và phải lớn hơn AP_NEED
        /// </summary>
        [SerializeField] private int _actionPoint;

        /// <summary>
        /// điểm kỹ năng cuối
        /// </summary>
        [SerializeField] private int _ultimatePoint;

        [SerializeField] private bool _isDead;
        public int FactionIndex => factionIndex;
        public int MemberIndex => memberIndex;
        public ElementType ElementType => _elementType;
        public Dictionary<AttributeType, int> Attributes => _attributes;
        public int ActionPoint => _actionPoint;
        public int UltimatePoint => _ultimatePoint;
        public bool IsDead => _isDead;
        public void ResetAP() => _actionPoint = 0;

        public void AccumulateAP()
        {
            if (IsDead) return;
            if (Attributes[AttributeType.AttackSpeed] <= 0)
            {
                throw new Exception("Speed must be greater than 0");
            }

            _actionPoint += Attributes[AttributeType.AttackSpeed];
        }

        public async UniTask SetupBattle(Card card, int factionIndexInBoard, int memberIndexInBoard)
        {
            factionIndex = factionIndexInBoard;
            memberIndex = memberIndexInBoard;
            _isDead = false;

            //element
            var cardTemplate = await Global.Instance.Get<GameConfig>().GetCardTemplate(card.CardModel.TemplateId);
            _elementType = cardTemplate.Element;

            //attribute
            _attributes.Clear();
            foreach (var attr in card.CardModel.CalculatedAttributes)
            {
                _attributes[attr.Type] = attr.Value;
            }

            SetupHp(AttributeType.Hp, AttributeType.HpMax);
            _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
            _vital.UpdateUp(0);
            _effect.PlaySpawn();
        }

        private void SetupHp(AttributeType current, AttributeType max)
        {
            if (_attributes.TryGetValue(max, out var maxAttr))
            {
                if (_attributes.ContainsKey(current))
                {
                    _attributes[current] = maxAttr; // Cập nhật nếu đã có
                }
                else
                {
                    _attributes[current] = maxAttr; // Thêm mới
                }
            }
        }

        public void AddUltimatePoint(int point = 1)
        {
            _ultimatePoint += point;
            if (_ultimatePoint > 100)
            {
                _ultimatePoint = 100;
            }

            if (_ultimatePoint < 0)
            {
                _ultimatePoint = 0;
            }
            _vital.UpdateUp(_ultimatePoint);
        }

        public (int damage, List<DamageLogType> logs) GetDamage()
        {
            var damageLogs = new List<DamageLogType>();
            int totalDamage = 0;
            int damage = _attributes[AttributeType.Attack];

            //calculated
            totalDamage += damage;
            //===================[Crit]=========================\\
            if (CalCrit(_attributes[AttributeType.CritChance], _attributes[AttributeType.CritDamage], ref totalDamage))
            {
                damageLogs.Add(DamageLogType.Crit);
            }

            if (totalDamage < 0) totalDamage = 0;
            return (totalDamage, damageLogs);
        }

        public (int aDamage, List<DamageLogType> logs) OnTakeDamage(int attackerTotalDamage, Card attacker)
        {
            var damageLogs = new List<DamageLogType>();
            if (_isDead) return (0, damageLogs);
            int victimTotalDefense = 0;
            victimTotalDefense += _attributes[AttributeType.Defense];

            //==================[ArmorPiercing]========================\\
            if (CalArmorPiercing(
                    attacker.Battle.Attributes[AttributeType.ArmorPenetrationChance],
                    attacker.Battle.Attributes[AttributeType.ArmorPenetrationDamage],
                    ref victimTotalDefense))
            {
                damageLogs.Add(DamageLogType.ArmorPenetration);
            }

            //=====================[Dodge]=============================\\
            if (CalDodge(_attributes[AttributeType.DodgeChance], _attributes[AttributeType.DodgeDamage],
                    ref attackerTotalDamage))
            {
                damageLogs.Add(DamageLogType.Dodge);
            }

            //=====================[Def]==============================\\
            CalDef(victimTotalDefense, ref attackerTotalDamage);

            //======================[HP]===========================\\
            _attributes[AttributeType.Hp] -= attackerTotalDamage;
            AddUltimatePoint((int)(GetHpLostPercentFromMax(attackerTotalDamage) * 2.25f));
            return (attackerTotalDamage, damageLogs);
        }
        private int GetHpLostPercentFromMax(float damageTaken)
        {
            float maxHp = _attributes[AttributeType.HpMax];
            if (maxHp <= 0f) return 0;
            return Mathf.FloorToInt(damageTaken / maxHp * 100f);
        }

        public bool OnTakeDamageLate()
        {
            if (_attributes[AttributeType.Hp] <= 0)
            {
                _isDead = true;
                _effect.PlayDie();
            }

            _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
            return _isDead;
        }
    }
}