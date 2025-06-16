using System;
using System.Collections.Generic;
using System.Linq;
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
            var levelBonus = Global.Instance.Get<GameConfig>()
                .CardLevelAttributeBonus(template.Class, Level.GetLevel());
            var starBonus = Global.Instance.Get<GameConfig>().CardStarAttributeBonus(template.Class, Star);

// Tập hợp tất cả các attribute type có thể có
            var allKeys = new HashSet<AttributeType>(
                templateAttributes.Keys
                    .Union(levelBonus.Keys)
                    .Union(starBonus.Keys)
            );

            foreach (var key in allKeys)
            {
                int finalValue = 0;

                if (templateAttributes.TryGetValue(key, out var baseValue))
                {
                    finalValue += baseValue;
                }

                if (levelBonus.TryGetValue(key, out var levelValue))
                {
                    finalValue += levelValue;
                }

                if (starBonus.TryGetValue(key, out var starValue))
                {
                    finalValue += starValue;
                }

                CalculatedAttributes.Add(new AttributeModel
                {
                    Type = key,
                    Value = finalValue
                });
            }
        }
    }
}