using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using World.Card.Skill;

namespace World.Card
{
    [Serializable]
    public class CardTemplateModel
    {
        public ushort Id;
        public string Name;
        public string History;
        public ClassType Class;
        /// <summary>
        /// Starter pack attribute when summoned
        /// </summary>
        public List<AttributeModel> Attributes;
        public List<SkillTemplateModel> Skills;
    }
}