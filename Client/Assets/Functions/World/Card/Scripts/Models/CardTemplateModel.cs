using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using World.Card.Skill;

namespace World.Card
{
    [Serializable]
    [CreateAssetMenu(fileName = "CardTemplateModel", menuName = "Data/CardTemplateModel", order = 1)]
    public class CardTemplateModel : ScriptableObject
    {
        public ushort Id;
        public string Name;
        public string History;
        public ClassType Class;

        /// <summary>
        /// Starter pack attribute when summoned
        /// </summary>
        public List<AttributeModel> Attributes;

        // thứ tự Passive1, Passive2, BasicSkill, AdvancedSkill, Ultimate
        public SkillTemplateModel PassiveSkill;
        public SkillTemplateModel PassiveSkill2;
        public SkillTemplateModel BasicSkill;
        public SkillTemplateModel AdvancedSkill;
        public SkillTemplateModel UltimateSkill;
    }
}