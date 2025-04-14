using UnityEngine;
using UnityEngine.Serialization;
using World.Card;
namespace World.Board
{
    [System.Serializable]
    public class BoardFactionPosition
    {
        /// <summary>
        /// Index bắt đầu từ 1 (get từ mảng cần -1), xếp theo bàn từ trái sang phải
        /// </summary>
        public int MemberIndex;
        /// <summary>
        /// vị trí trên canvas
        /// </summary>
        public Vector2 OriginalPosition;
        /// <summary>
        /// ref
        /// </summary>
        public Card.Card Card;
    }
}