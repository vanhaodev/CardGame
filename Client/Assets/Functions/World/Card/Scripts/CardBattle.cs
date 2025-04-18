using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using World.Card;
using Random = UnityEngine.Random;

namespace World.Card
{
    public partial class CardBattle : MonoBehaviour
    {
        [SerializeField] private CardVital _vital;
        public CardVital Vital => _vital;
        [SerializeField] CardEffect _effect;
        //----------------------Data-------------------------\\
        [FormerlySerializedAs("_factionIndexInBoard")] [SerializeField] int factionIndex;
        [FormerlySerializedAs("_memberIndexInBoard")] [SerializeField] int memberIndex;
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
        public Dictionary<AttributeType, int> Attributes => _attributes;
        public int ActionPoint => _actionPoint;
        public int UltimatePoint => _ultimatePoint;
        public bool IsDead => _isDead;
        public void ResetAP() => _actionPoint = 0;
        public void AccumulateAP()
        {
            if(IsDead) return;
            if (Attributes[AttributeType.AttackSpeed] <= 0)
            {
                throw new Exception("Speed must be greater than 0");
            }

            _actionPoint += Attributes[AttributeType.AttackSpeed];
        }
        public void SetupBattle(Card card, int factionIndexInBoard, int memberIndexInBoard)
        {
            factionIndex = factionIndexInBoard;
            memberIndex = memberIndexInBoard;
            _isDead = false;
            // Xóa dữ liệu cũ để tránh lỗi trùng key
            _attributes.Clear();

            // Chuyển List thành Dictionary thủ công
            foreach (var attr in card.CardModel.CalculatedAttributes)
            {
                _attributes[attr.Type] = attr.Value;
            }

            SetupHpMp(AttributeType.Hp, AttributeType.HpMax);
            _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
            _effect.PlaySpawn();
        }

        private void SetupHpMp(AttributeType current, AttributeType max)
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

        public  (int aDamage, List<DamageLogType> logs) OnTakeDamage(int attackerTotalDamage, Card attacker)
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
            return (attackerTotalDamage, damageLogs);
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