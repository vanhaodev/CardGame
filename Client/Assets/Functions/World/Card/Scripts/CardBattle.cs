using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using Newtonsoft.Json;
using Popups;
using UniRx;
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
        [SerializeField] Card _card;
        public Card Card => _card;
        [SerializeField] CardEffect _effect;

        //----------------------Data-------------------------\\
        [FormerlySerializedAs("_factionIndexInBoard")] [SerializeField]
        int factionIndex;

        [FormerlySerializedAs("_memberIndexInBoard")] [SerializeField]
        int memberIndex;

        [SerializeField] private ElementType _elementType;
        [SerializeField] private SerializedDictionary<AttributeType, int> _attributes;
        [SerializeField] private SerializedDictionary<BattleAttributeType, int> _battleAttributes;

        [SerializeField] private bool _isDead;
        [SerializeField] private GameObject _ultimateableNotification;
        public int FactionIndex => factionIndex;
        public int MemberIndex => memberIndex;
        public ElementType ElementType => _elementType;
        public Dictionary<AttributeType, int> Attributes => _attributes;
        public Dictionary<BattleAttributeType, int> BattleAttributes => _battleAttributes;
        public bool IsDead => _isDead;
        public void ResetAP() => _battleAttributes[BattleAttributeType.ActionPoint] = 0;
        //============================[EVENT]==============================\\
        private readonly Subject<CardBattle> _eventOnTouch = new Subject<CardBattle>();
        public void InvokeEventOnTouch() => _eventOnTouch.OnNext(this);
        private IDisposable _onTouchListener;

        public void ListenEventOnTouch(Action<CardBattle> action)
        {
            _onTouchListener?.Dispose();
            _onTouchListener = _eventOnTouch.Subscribe(action).AddTo(this);
        }
        
        //============================[]==============================\\
        public void AccumulateAP()
        {
            if (IsDead) return;
            if (Attributes[AttributeType.Speed] <= 0)
            {
                throw new Exception("Speed must be greater than 0");
            }

            _battleAttributes[BattleAttributeType.ActionPoint] += Attributes[AttributeType.Speed];
        }

        public async UniTask SetupBattle(CardModel cardModel, int factionIndexInBoard, int memberIndexInBoard)
        {
            _card.CardModel = cardModel;
            factionIndex = factionIndexInBoard;
            memberIndex = memberIndexInBoard;
            _isDead = false;

            //element
            var cardTemplate = await Global.Instance.Get<GameConfig>().GetCardTemplate(_card.CardModel.TemplateId);
            _elementType = cardTemplate.Element;

            //pre creat attribute
            foreach (BattleAttributeType type in Enum.GetValues(typeof(BattleAttributeType)))
            {
               _battleAttributes[type] = 0;
            }
            
            //attribute
            _attributes.Clear();
            foreach (var attr in _card.CardModel.CalculatedAttributes)
            {
                _attributes[attr.Type] = attr.Value;
            }

            SetupHp();
            _vital.UpdateHp(_battleAttributes[BattleAttributeType.Hp], _attributes[AttributeType.HpMax]);
            _vital.UpdateUp(0);
            _effect.PlaySpawn();
        }

        private void SetupHp()
        {
            if (_attributes.TryGetValue(AttributeType.HpMax, out var maxAttr))
            {
                if (_battleAttributes.ContainsKey(BattleAttributeType.Hp))
                {
                    _battleAttributes[BattleAttributeType.Hp] = maxAttr; // Cập nhật nếu đã có
                }
                else
                {
                    _battleAttributes[BattleAttributeType.Hp] = maxAttr; // Thêm mới
                }
            }
        }

        public void AddUltimatePoint(int point = 1)
        {
            _battleAttributes[BattleAttributeType.UltimatePoint] += point;
            if (_battleAttributes[BattleAttributeType.UltimatePoint] > 100)
            {
                _battleAttributes[BattleAttributeType.UltimatePoint] = 100;
            }

            if (_battleAttributes[BattleAttributeType.UltimatePoint] < 0)
            {
                _battleAttributes[BattleAttributeType.UltimatePoint] = 0;
            }

            _vital.UpdateUp(_battleAttributes[BattleAttributeType.UltimatePoint]);
            _ultimateableNotification.SetActive(_battleAttributes[BattleAttributeType.UltimatePoint] >= 100);
        }

        public (int damage, List<DamageLogType> logs) GetDamage()
        {
            var damageLogs = new List<DamageLogType>();
            int totalDamage = 0;
            int damage = _attributes[AttributeType.Attack];

            //calculated
            totalDamage += damage;
            //===================[Crit]=========================\\
            if (CalCrit(_attributes[AttributeType.CriticalRate], _attributes[AttributeType.CriticalDamage],
                    ref totalDamage))
            {
                damageLogs.Add(DamageLogType.Crit);
            }

            if (totalDamage < 0) totalDamage = 0;
            return (totalDamage, damageLogs);
        }

        public (int aDamage, List<DamageLogType> logs) OnTakeDamage(int attackerTotalDamage, CardBattle attacker)
        {
            var damageLogs = new List<DamageLogType>();
            if (_isDead) return (0, damageLogs);
            int victimTotalDefense = 0;
            victimTotalDefense += _attributes[AttributeType.Defense];

            //==================[ArmorPiercing]========================\\
            if (CalArmorPiercing(
                    attacker.Attributes[AttributeType.ArmorPenetrationChance],
                    attacker.Attributes[AttributeType.ArmorPenetrationDamage],
                    ref victimTotalDefense))
            {
                damageLogs.Add(DamageLogType.ArmorPenetration);
            }

            //=====================[Dodge]=============================\\
            if (CalDodge(_attributes[AttributeType.DodgeRate], _attributes[AttributeType.DodgeDamage],
                    ref attackerTotalDamage))
            {
                damageLogs.Add(DamageLogType.Dodge);
            }

            //=====================[Def]==============================\\
            CalDef(victimTotalDefense, ref attackerTotalDamage);

            //======================[HP]===========================\\
            _battleAttributes[BattleAttributeType.Hp] -= attackerTotalDamage;
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
            if (_battleAttributes[BattleAttributeType.Hp] <= 0)
            {
                _isDead = true;
                _effect.PlayDie();
            }

            _vital.UpdateHp(_battleAttributes[BattleAttributeType.Hp], _attributes[AttributeType.HpMax]);
            return _isDead;
        }
        
        /// <summary>
        /// if not use skill or attack target => Show this card's info <br/>
        /// if use skill, heal, buff or attack command => this will be the taraget
        /// </summary>
        public void OnTouch()
        {
            InvokeEventOnTouch();
        }

        public void OnHold()
        {
            Debug.Log(JsonConvert.SerializeObject(_card.CardModel));
            Global.Instance.Get<PopupManager>().ShowCard(new PopupCardBattleModel()
            {
                CardModel = _card.CardModel,
            });
        }
    }
}