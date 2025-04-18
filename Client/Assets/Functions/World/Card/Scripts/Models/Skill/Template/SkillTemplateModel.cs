using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using World.Requirement;

namespace World.Card.Skill
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Data/Skill/Skill")]
    public class SkillTemplateModel : ScriptableObject
    {
        public ushort Id;
        public string Name;
        public string Description;
        /// <summary>
        /// một skill có thể vừa là bị động, vừa đánh đc địch vừa làm lợi cho đội
        /// </summary>
        public List<SkillType> SkillTypes;
        /// <summary>
        /// max 5 stars
        /// </summary>
        public List<SkillBuilderTemplateModel> Levels;
    }
}