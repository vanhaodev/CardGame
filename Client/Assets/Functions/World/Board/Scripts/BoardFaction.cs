using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Board
{
    public class BoardFaction : MonoBehaviour
    {
        [System.Serializable]
        public class Position
        {
            public int Index; // Đổi từ byte sang int
            public Vector2 OriginalPosition;
            public GameObject Card;
        }

        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
        [SerializeField] private List<Position> Positions;

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