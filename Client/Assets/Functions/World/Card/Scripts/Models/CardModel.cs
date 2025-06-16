using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using GameConfigs;
using Globals;
using NUnit.Framework;
using Sirenix.OdinInspector;
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

        [Button]
        /// <summary>
        /// Cập nhật khi cường hoá hoặc phiên bản mới
        /// </summary>
        public async UniTask UpdateAttribute()
        {
            // Lấy template (có thể thay đổi mỗi bản cập nhật)
            var template = await Global.Instance.Get<GameConfig>().GetCardTemplate(TemplateId);

            // Nếu template null thì log warning và thoát
            if (template == null)
            {
                throw new Exception($"Card template with ID {TemplateId} is null or not an equipment template.");
                return;
            }

            CalculatedAttributes = new List<AttributeModel>();
            var templateAttributes = AttributeModel.ToDictionary(template.Attributes);
            //bonus từ cấp độ
            float levelPercentBonus =
                Global.Instance.Get<GameConfig>().CardLevelAttributePercentBonus(Level.GetLevel());
            //bonus từ star
            var starAttributePercentBonus =
                Global.Instance.Get<GameConfig>().CardStarAttributePercentBonus(Star);
            foreach (var attribute in templateAttributes)
            {
                int finalValue = 0;
                finalValue += attribute.Value + (int)(attribute.Value * levelPercentBonus / 100f);
                finalValue += attribute.Value + (int)(attribute.Value * starAttributePercentBonus / 100f);

                CalculatedAttributes.Add(new AttributeModel
                {
                    Type = attribute.Key,
                    Value = finalValue
                });
            }
        }
    }
}