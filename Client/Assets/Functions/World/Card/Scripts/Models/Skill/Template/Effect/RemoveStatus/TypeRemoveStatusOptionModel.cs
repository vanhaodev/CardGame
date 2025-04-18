using System.Collections.Generic;
using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TypeRemoveStatusOption", menuName = "Data/Skill/Effect/SkillRemoveStatusOption/TypeRemoveStatus")]
    public class TypeRemoveStatusOptionModel : RemoveStatusOptionModel
    {
        public byte QuantityMax;
        public List<string> Types = new List<string>(); //frezee, burn...
    }
}