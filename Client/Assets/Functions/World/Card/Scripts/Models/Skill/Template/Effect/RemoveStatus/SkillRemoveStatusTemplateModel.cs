using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace World.Card.Skill
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SkillRemoveStatus", menuName = "Data/Skill/Effect/SkillRemoveStatus")]
    public class SkillRemoveStatusTemplateModel: SkillEffectTemplateModel
    {
        public short Rate;
        /// <summary>
        /// xoá trạng thái tốt
        /// </summary>
        public bool IsRemoveBuff;
        /// <summary>
        /// Xoá trạng thái xấu
        /// </summary>
        public bool IsRemoveDebuff;
        public List<RemoveStatusOptionModel> Options;
    }
}