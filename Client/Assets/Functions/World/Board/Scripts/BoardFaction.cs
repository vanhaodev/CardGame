using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Board
{
    public class BoardFaction : MonoBehaviour
    {
        [System.Serializable]
        public struct Position
        {
            public int Index; // Đổi từ byte sang int
            public GameObject ObjCard;
        }

        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
        [SerializeField] private List<Position> Positions;
        /// <summary>
        /// When card move to attack, lerp to index with root pos
        /// </summary>
        private Dictionary<int, Vector2> _cardRootPositions = new Dictionary<int, Vector2>();

        public void SetRootPositions(int index, Vector2 position)
        {
            _cardRootPositions[index] = position;
        }

        public Vector2 GetRootPosition(int index)
        {
            if (_cardRootPositions.TryGetValue(index, out var position)) return position;
            return Vector2.zero;
        }

        public List<Position> GetPositions()
        {
            return Positions;
        }
    }
}