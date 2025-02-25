using System.Collections.Generic;
using UnityEngine;

namespace World.Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] GameObject _prefabCard;
        /// <summary>
        /// Factions to battle, if 2 factions => 1vs1 board, 1vs1vs1 => 1vs1vs1 board <br/>
        /// Current just 1vs1 top and bottom with vertical screen <br/>
        /// [0] is faction 1 (bottom) | [1] is faction 2 (top)
        /// </summary>
        [SerializeField] List<BoardFaction> _objFactions;

        /// <summary>
        /// test function for change screen size in runtime
        /// </summary>
        [Sirenix.OdinInspector.Button]
        void RefreshCardPosition()
        {
            
        }
    }
}