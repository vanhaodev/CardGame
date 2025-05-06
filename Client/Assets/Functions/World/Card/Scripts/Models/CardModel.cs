using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using World.TheCard.Skill;

namespace World.TheCard
{
    /// <summary>
    /// chứa thông tin unique của 1 card
    /// </summary>
    [Serializable]
    public class CardModel
    {
        public ushort TemplateId;

        /// <summary>
        /// Bậc sao của card, bậc càng cao, ảnh càng đẹp
        /// </summary>
        public byte Star;

        /// <summary>
        /// Cấp của card
        /// </summary>
        public CardLevelModel Level;

        /// <summary>
        /// Base is default stats and star, level, enchantic stats...
        /// </summary>
        public List<AttributeModel> BaseAttributes = new List<AttributeModel>();

        /// <summary>
        /// Calculated = equipments + base
        /// </summary>
        public List<AttributeModel> CalculatedAttributes = new List<AttributeModel>();
        
        // thứ tự Passive1, Passive2, BasicSkill, AdvancedSkill, Ultimate
        public SkillModel PassiveSkill;
        public SkillModel PassiveSkill2;
        public SkillModel BasicSkill;
        public SkillModel AdvancedSkill;
        public SkillModel UltimateSkill;
    }
}