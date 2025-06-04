using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using Sirenix.OdinInspector;
using World.TheCard;

namespace Functions.World.Player.Inventory
{
    [System.Serializable]
    public class ItemEquipmentModel : ItemModel
    {
        /// <summary>
        /// Cường hoá + <br/>
        /// Mỗi cấp cộng 0.25% điểm att đang có
        /// </summary>
        public byte UpgradeLevel = 1;

        /// <summary>
        /// tăng chỉ số static vào người đeo
        /// </summary>
        public List<AttributeModel> Attributes;

        public List<AttributeModel> CalculatedAttributes;

        /// <summary>
        /// Tăng chỉ số người đeo theo %
        /// </summary>
        public List<AttributeModel> AttributePercents;

        public List<AttributeModel> CalculatedAttributePercents;

        [Button]
        /// <summary>
        /// Cập nhật khi cường hoá hoặc phiên bản mới
        /// </summary>
        public async UniTask UpdateAttribute()
        {
            // Lấy template (có thể thay đổi mỗi bản cập nhật)
            var template =
                await Global.Instance.Get<GameConfig>().GetItemTemplate(TemplateId) as ItemEquipmentTemplateModel;
            var templateAttributes = AttributeModel.ToDictionary(template.Attributes);
            var templateAttributePercents = AttributeModel.ToDictionary(template.AttributePercents);

            // Lấy bonus phần trăm từ cấp cường hóa (0.25f → 25%)
            float upgradePercentBonus =
                Global.Instance.Get<GameConfig>().UpgradeItemPercentBonus(UpgradeLevel); // ví dụ 26f nghĩa là 26%

            CalculatedAttributes = new List<AttributeModel>();

            // Duyệt tất cả các type có trong instance hoặc template để tính toán chỉ số
            var allAttrTypes = Attributes.Select(a => a.Type)
                .Union(templateAttributes.Keys);

            foreach (var type in allAttrTypes)
            {
                float baseValue = 0;

                // Lấy từ instance
                var fromInstance = Attributes.FirstOrDefault(a => a.Type == type);
                baseValue += fromInstance?.Value ?? 0;

                // Lấy từ template
                if (templateAttributes.TryGetValue(type, out var templateValue))
                {
                    baseValue += templateValue;
                }

                // Cộng bonus từ cường hóa
                float upgradedValue = baseValue + (baseValue * upgradePercentBonus / 100f);

                CalculatedAttributes.Add(new AttributeModel
                {
                    Type = type,
                    Value = (int)upgradedValue // Ép về int cho consistent
                });
            }

            // Tương tự cho phần trăm (dùng hệ 10000 thay vì %)
            CalculatedAttributePercents = new List<AttributeModel>();

            var allPercentAttrTypes = AttributePercents.Select(a => a.Type)
                .Union(templateAttributePercents.Keys);

            foreach (var type in allPercentAttrTypes)
            {
                int baseValue = 0;

                var fromInstance = AttributePercents.FirstOrDefault(a => a.Type == type);
                baseValue += fromInstance?.Value ?? 0;

                if (templateAttributePercents.TryGetValue(type, out var templatePercentValue))
                {
                    baseValue += templatePercentValue;
                }

                // Bonus phần trăm cũng tính như thường, nhưng giữ hệ số 10000
                float upgradedPercent = baseValue + (baseValue * upgradePercentBonus / 100f);

                CalculatedAttributePercents.Add(new AttributeModel
                {
                    Type = type,
                    Value = (int)upgradedPercent // vẫn dùng int vì hệ 10000
                });
            }
        }
    }
}