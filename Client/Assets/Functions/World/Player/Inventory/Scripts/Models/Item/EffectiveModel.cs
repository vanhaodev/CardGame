using System;
using System.Collections.Generic;
using System.Linq;
using World.Card;

namespace World.Player.Inventory
{
    public class EffectiveModel
    {
        public byte? MaxSuccessEffective; // If Eff list has rate, the effect success never greater than max

        public List<EffectiveDetailModel> Effectives;

        /// <summary>
        /// Sort when create class, need sort for random from lowest to highest chance;
        /// </summary>
        public void SortBySuccessRate()
        {
            // Sắp xếp danh sách Effectives theo SuccessRate từ thấp đến cao (nếu SuccessRate có giá trị)
            Effectives = Effectives.Where(e => e.SuccessRate.HasValue).OrderBy(e => e.SuccessRate).ToList();
        }

        /// <summary>
        /// Random and return a list of effective effects based on rules
        /// </summary>
        /// <returns></returns>
        public List<EffectiveDetailModel> GetEffectiveList()
        {
            List<EffectiveDetailModel> result = new List<EffectiveDetailModel>();

            // 1. Lấy ra các hiệu quả có SuccessRate null
            var nullRateEffectives = Effectives.Where(e => !e.SuccessRate.HasValue).ToList();

            // 2. Lọc ra các hiệu quả còn lại có SuccessRate không null
            var applicableEffectives = Effectives.Where(e => e.SuccessRate.HasValue).ToList();

            // 3. Nếu MaxSuccessEffective null thì random hết cái list còn lại
            if (!MaxSuccessEffective.HasValue)
            {
                result = applicableEffectives.OrderBy(x => Guid.NewGuid()).ToList();
            }
            else
            {
                // 4. Nếu MaxSuccessEffective not null thì random cho đến khi success được số lượng bằng max
                int successCount = 0;
                var random = new Random();
                while (successCount < MaxSuccessEffective.Value && applicableEffectives.Any())
                {
                    var randomEffect = applicableEffectives[random.Next(applicableEffectives.Count)];
                    if (randomEffect.SuccessRate.HasValue && randomEffect.SuccessRate.Value > random.Next(10000))
                    {
                        result.Add(randomEffect);
                        successCount++;
                    }

                    applicableEffectives.Remove(randomEffect);
                }

                // 5. Nếu không đủ số lượng thành công (successCount < MaxSuccessEffective), lấy ít hơn max vẫn chấp nhận
                if (successCount < MaxSuccessEffective.Value)
                {
                    result = result.Concat(applicableEffectives).ToList();
                }
            }

            // 6. Kết hợp các hiệu quả có SuccessRate null vào kết quả cuối cùng
            result = result.Concat(nullRateEffectives).ToList();

            return result;
        }
    }

    public class EffectiveDetailModel
    {
        public ushort? SuccessRate; // Success rate in 1/10000, fail show center alert
        public EffectiveAttributeModel? Attribute;
        public EffectiveSkillModel? Skill;
        public EffectiveRewardModel? Reward;
    }

    public class EffectiveAttributeModel
    {
        public AttributeType AttributeType;
        public int? Value;
        public int? PercentValue;
        public float? EffectiveTimeSeconds; // null => forever (like HP recovery)
    }

    public class EffectiveSkillModel
    {
        public ushort SkillId;
        public byte UpgradeLevel;
    }

    public class EffectiveRewardModel
    {
        public RewardModel Reward;
    }
}