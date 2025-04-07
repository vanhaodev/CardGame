using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using World.Card.Skill;

namespace World.Card
{
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
        public int Exp;
        /// <summary>
        /// Base is default stats and star, level, enchantic stats...
        /// </summary>
        public List<AttributeModel> BaseAttributes = new List<AttributeModel>();
        /// <summary>
        /// Calculated = equipments + base
        /// </summary>
        public List<AttributeModel> CalculatedAttributes = new List<AttributeModel>();
        public List<SkillModel> Skills = new List<SkillModel>();
    } 
}