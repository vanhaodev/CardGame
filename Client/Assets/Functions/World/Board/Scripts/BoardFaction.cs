using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using World.Card;
using Random = UnityEngine.Random;

namespace World.Board
{
    public class BoardFaction : MonoBehaviour
    {
        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
        [SerializeField] private List<BoardFactionPosition> _positions;

        private void Start()
        {
            foreach (var position in _positions)
            {
                var testModel = new CardModel();
                testModel.TemplateId = (ushort)Random.Range(1, 3);
                testModel.Star = (byte)Random.Range(1, 6);
                foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
                {
                    if ((int)type < 2)
                    {
                        testModel.CalculatedAttributes.Add(new AttributeModel
                        {
                            Type = type,
                            Value = 10
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

                position.Card.CardModel = testModel;
            }
        }

        public List<BoardFactionPosition> GetPositions() => _positions;

        public BoardFactionPosition GetPosition(int index)
        {
            return _positions[index - 1];
        }

        [Button]
        private void InitOriginalPosition()
        {
            foreach (var p in _positions)
            {
                var rect = p.Card.GetComponent<RectTransform>();
                p.OriginalPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
            }
        }

        [Button]
        private void ResetToOriginalPosition()
        {
            foreach (var p in _positions)
            {
                var rect = p.Card.GetComponent<RectTransform>();
                rect.anchoredPosition = p.OriginalPosition;
            }
        }
        public bool IsAllDead()
        {
            return _positions.All(i => i.Card.Battle.IsDead); // Tất cả đều chết thì trả về true
        }
    }
}