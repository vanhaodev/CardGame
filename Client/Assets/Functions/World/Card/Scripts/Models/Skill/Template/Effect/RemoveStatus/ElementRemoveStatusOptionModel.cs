using System.Collections.Generic;
using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class ElementRemoveStatusOptionModel : RemoveStatusOptionModel
    {
        public byte QuantityMax;
        public List<ElementType> Elements = new List<ElementType>(); //water, earth...
    }
}