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
        public uint Id;
        public uint TemplateId;

        /// <summary>
        /// Awakening Stage của card, bậc càng cao, ảnh càng đẹp chỉ số càng bá, skill nâng cấp cao hơn, gọi là Awakening Stage
        /// </summary>
        public byte Star;

        /// <summary>
        /// Cấp của card
        /// </summary>
        public CardLevelModel Level;
        /// <summary>
        /// Calculated = equipments + base + template base
        /// <br/>
        /// Vì sao không cộng luôn tempkate vào base mà phải cộng sau ở final? vì sau này game có thể thay đỏi template attribute, lúc đó card cũ vẫn có thể có chỉ số mới
        /// </summary>
        public List<AttributeModel> CalculatedAttributes = new List<AttributeModel>();
        
        // thứ tự Passive1, Passive2, BasicSkill, AdvancedSkill, Ultimate
        /// <summary>
        /// Nếu skill ở temp ko có ở trong đây thì mặc định là level 1
        /// </summary>
        [SerializeReference]
        public Dictionary<CardSkillSlotType, SkillModel> Skills = new Dictionary<CardSkillSlotType, SkillModel>();
    }
}