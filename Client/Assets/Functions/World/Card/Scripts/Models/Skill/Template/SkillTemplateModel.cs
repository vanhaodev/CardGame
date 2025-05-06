using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using World.Requirement;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillTemplateModel
    {
        public string Name;
        public string Description;
        public SkillTargetType TargetType;
        /// <summary>
        /// max 5 stars
        /// </summary>
        public List<SkillOptionTemplateModel> Levels;
    }
}