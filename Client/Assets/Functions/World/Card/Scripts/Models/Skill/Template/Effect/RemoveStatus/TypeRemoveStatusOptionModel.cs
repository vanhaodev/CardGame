using System.Collections.Generic;
using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    public class TypeRemoveStatusOptionModel : RemoveStatusOptionModel
    {
        public byte QuantityMax;
        public List<string> Types = new List<string>(); //frezee, burn...
    }
}