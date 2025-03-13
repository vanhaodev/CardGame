using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Card
{
    [Serializable]
    public class CardModel
    {
        public ushort TemplateId;
        public byte Rank;
        public int Exp;
        /// <summary>
        /// Base is default stats and star, level, enchantic stats...
        /// </summary>
        public List<AttributeModel> BaseAttributes = new List<AttributeModel>();
        /// <summary>
        /// Calculated = equipments + base
        /// </summary>
        public List<AttributeModel> CalculatedAttributes = new List<AttributeModel>();
        
    } 
}