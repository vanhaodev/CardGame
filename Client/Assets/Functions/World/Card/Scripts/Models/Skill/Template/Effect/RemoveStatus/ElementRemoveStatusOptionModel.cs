using System.Collections.Generic;
using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ElementRemoveStatusOption", menuName = "Data/Skill/Effect/SkillRemoveStatusDetail/ElementRemoveStatusOption")]
    public class ElementRemoveStatusOptionModel : RemoveStatusOptionModel
    {
        public byte QuantityMax;
        public List<ElementType> Elements = new List<ElementType>(); //water, earth...
    }
}