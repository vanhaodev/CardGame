using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Board
{
    public class BoardFaction : MonoBehaviour
    {
        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
        [SerializeField] private List<BoardFactionPosition> Positions;

        public BoardFactionPosition GetPosition(int index)
        {
            return Positions[index-1];
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