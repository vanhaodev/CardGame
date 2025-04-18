using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using World.Requirement;

namespace World.Card.Skill
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Data/Skill/Skill")]
    public class SkillTemplateModel : ScriptableObject
    {
        public string Name;
        public string Description;
        /// <summary>
        /// 
        /// </summary>
        public SkillType SkillType;
        /// <summary>
        /// max 5 stars
        /// </summary>
        public List<SkillOptionTemplateModel> Levels;
    }
}