using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Card;
using Random = UnityEngine.Random;

namespace World.Board
{
    public class BoardFaction : MonoBehaviour
    {
        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
        [SerializeField] private List<BoardFactionPosition> Positions;

        private void Start()
        {
            foreach (var position in Positions)
            {
                var testModel = new CardModel();
                testModel.TemplateId = (ushort)Random.Range(1, 3);
                testModel.Rank = (byte)Random.Range(1, 6);
                foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
                {
                    testModel.CalculatedAttributes.Add(new AttributeModel
                    {
                        Type = type,
                        Value = Random.Range(1, 500)
                    });
                }

                position.Card.CardModel = testModel;
            }
        }

        public BoardFactionPosition GetPosition(int index)
        {
            return Positions[index - 1];
        }

        [Button]
        private void InitOriginalPosition()
        {
            foreach (var p in Positions)
            {
                var rect = p.Card.GetComponent<RectTransform>();
                p.OriginalPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
            }
        }

        [Button]
        private void ResetToOriginalPosition()
        {
            foreach (var p in Positions)
            {
                var rect = p.Card.GetComponent<RectTransform>();
                rect.anchoredPosition = p.OriginalPosition;
            }
        }
    }
}