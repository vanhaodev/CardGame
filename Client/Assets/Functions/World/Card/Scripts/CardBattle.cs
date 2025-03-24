using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using World.Card;
using Random = UnityEngine.Random;

namespace World.Card
{
    public partial class CardBattle : MonoBehaviour
    {
        [SerializeField] private bool _isDead;
        public bool IsDead => _isDead;

        [SerializeField] private SerializedDictionary<AttributeType, int> _attributes;
        [SerializeField] private CardVital _vital;
        public CardVital Vital => _vital;
        [SerializeField] CardEffect _effect;
        public Dictionary<AttributeType, int> Attributes => _attributes;

        public void SetupBattle(Card card)
        {
            _isDead = false;
            // Xóa dữ liệu cũ để tránh lỗi trùng key
            _attributes.Clear();

            // Chuyển List thành Dictionary thủ công
            foreach (var attr in card.CardModel.CalculatedAttributes)
            {
                _attributes[attr.Type] = attr.Value;
            }

            SetupHpMp(AttributeType.Hp, AttributeType.HpMax);
            SetupHpMp(AttributeType.Mp, AttributeType.MpMax);
            _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
            _vital.UpdateMp(_attributes[AttributeType.Mp], _attributes[AttributeType.MpMax]);
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

        public void OnTakeDamageLate()
        {
            if (_attributes[AttributeType.Hp] <= 0)
            {
                _isDead = true;
                _effect.PlayDie();
            }

            _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
        }
    }
}