using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;

namespace World.Card
{
    [Serializable]
    public class CardTemplateModel
    {
        public ushort Id;
        public string Name;
        public string History;
        public List<AttributeModel> Attributes;
    }
}