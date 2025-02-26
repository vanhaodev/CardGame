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
            public int Index;  // Đổi từ byte sang int
            public GameObject ObjCard;
        }
        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
        [SerializeField] private List<Position> Positions;

        public List<Position> GetPositions()
        {
            return Positions;
        }
    }
}