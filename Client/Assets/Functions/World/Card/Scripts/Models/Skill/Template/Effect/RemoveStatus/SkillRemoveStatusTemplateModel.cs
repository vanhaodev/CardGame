using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillRemoveStatusTemplateModel: SkillEffectTemplateModel
    {
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