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
        public List<AttributeModel> BaseAttributes = new List<AttributeModel>();
        public List<AttributeModel> CalculatedAttributes = new List<AttributeModel>();
    } 
}