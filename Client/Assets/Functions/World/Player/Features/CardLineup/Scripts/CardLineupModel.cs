using System;
using System.Collections.Generic;
using UnityEngine;
using World.TheCard;

namespace Functions.World.Player
{
    [Serializable]
    /// <summary>
    /// Có thể setup trước lineup cho nhiều đội hình giúp khắc chế tốt hơn
    /// </summary>
    public class CardLineupModel
    {
        [SerializeReference]
        public Dictionary<byte /*slot index start from 0*/, uint /*card unique id*/> Cards;
    }
}