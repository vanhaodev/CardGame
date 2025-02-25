using System.Collections.Generic;
using UnityEngine;

namespace World.Board
{
    public class Board
    {
        /// <summary>
        /// Factions to battle, if 2 factions => 2vs2 board, 1vs1vs1 => 1vs1vs1 board
        /// </summary>
        [SerializeField] List<BoardFaction> _objFactions;
        
    }
}