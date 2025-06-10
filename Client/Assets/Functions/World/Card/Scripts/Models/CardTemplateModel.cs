using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Globals;
using UnityEngine;
using World.TheCard.Skill;

namespace World.TheCard
{
    [Serializable]
    [CreateAssetMenu(fileName = "CardTemplateModel", menuName = "Data/CardTemplateModel", order = 1)]
    public class CardTemplateModel : ScriptableObject
    {
        public uint Id;
        public string Name;
        public string History;
        /// <summary>
        /// ItemtemplateId
        /// </summary>
        public uint ShardId;
        public ClassType Class;
        public ElementType Element;
        /// <summary>
        /// Starter pack attribute when summoned
        /// </summary>
        public List<AttributeModel> Attributes;

        // thứ tự Passive1, Passive2, BasicSkill, AdvancedSkill, Ultimate
        public List<CardSkillTemplateModel> Skills;
    }
}