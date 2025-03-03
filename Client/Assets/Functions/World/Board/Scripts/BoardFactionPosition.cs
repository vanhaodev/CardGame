using UnityEngine;
using World.Card;
namespace World.Board
{
    [System.Serializable]
    public class BoardFactionPosition
    {
        public int Index; // Đổi từ byte sang int
        public Vector2 OriginalPosition;
        public Card.Card Card;
    }
}