using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using World.TheCard;
using World.TheCard.Skill;
using Random = UnityEngine.Random;

namespace World.Board
{
    public class BoardFaction : MonoBehaviour
    {
        /// <summary>
        /// các battler của phe
        /// </summary>
        [SerializeField] private List<BoardFactionPosition> _positions;

        [SerializeField] private SerializedDictionary<FactionAttributeType, int> _factionAttributes;
        public SerializedDictionary<FactionAttributeType, int> FactionAttributes => _factionAttributes;

        private void Start()
        {
            //pre creat attribute
            foreach (FactionAttributeType type in Enum.GetValues(typeof(FactionAttributeType)))
            {
                _factionAttributes[type] = 0;
            }

            //test chỉ số
            foreach (var position in _positions)
            {
                var testModel = new CardModel();
                testModel.TemplateId = (ushort)Random.Range(1, 3);
                testModel.Star = (byte)Random.Range(1, 6);
                testModel.Level = new CardLevelModel();
                testModel.Level.SetExp((uint)Random.Range(1, 4000));
                foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
                {
                    if ((int)type < 2)
                    {
                        testModel.CalculatedAttributes.Add(new AttributeModel
                        {
                            Type = type,
                            Value = 200
                        });
                    }
                    else if ((int)type < 8)
                    {
                        testModel.CalculatedAttributes.Add(new AttributeModel
                        {
                            Type = type,
                            Value = Random.Range(200, 500)
                        });
                    }
                    else //chance 
                    {
                        testModel.CalculatedAttributes.Add(new AttributeModel
                        {
                            Type = type,
                            Value = Random.Range(1000, 10000)
                        });
                    }
                }

                // testModel.Skills =
                position.CardBattle.Card.CardModel = testModel;
            }
        }

        public List<BoardFactionPosition> GetAllPositions() => _positions;

        public BoardFactionPosition GetPositionByIndex(int index)
        {
            // Debug.Log($"GetPositionByIndex: {index}");
            return _positions[index - 1];
        }

        [Button]
        private void InitOriginalPosition()
        {
            foreach (var p in _positions)
            {
                var rect = p.CardBattle.GetComponent<RectTransform>();
                p.OriginalPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
            }
        }

        [Button]
        public void ResetToOriginalPosition()
        {
            foreach (var p in _positions)
            {
                var rect = p.CardBattle.GetComponent<RectTransform>();
                rect.anchoredPosition = p.OriginalPosition;
                rect.localRotation = Quaternion.identity;
            }
        }

        public void AddSkillPoint(int point = 1)
        {
            _factionAttributes[FactionAttributeType.SkillPoint] += point;
            if (_factionAttributes[FactionAttributeType.SkillPoint] > 5)
            {
                _factionAttributes[FactionAttributeType.SkillPoint] = 5;
            }

            if (_factionAttributes[FactionAttributeType.SkillPoint] < 0)
            {
                _factionAttributes[FactionAttributeType.SkillPoint] = 0;
            }
        }

        /// <summary>
        /// Đội đã die hết
        /// </summary>
        /// <returns></returns>
        public bool IsAllDead()
        {
            return _positions.All(i => i.CardBattle.IsDead); // Tất cả đều chết thì trả về true
        }
    }
}