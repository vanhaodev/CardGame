using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RamdomRemoveStatusOption", menuName = "Data/Skill/Effect/SkillRemoveStatusDetail/RandomRemoveStatusOption")]
    public class RandomRemoveStatusOptionModel : RemoveStatusOptionModel
    {
        public byte QuantityMax;
    }
}