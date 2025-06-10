using System.Collections.Generic;

namespace Functions.World.Gacha
{
    [System.Serializable]
    public class GachaRewardModel
    {
        /// <summary>
        /// 100k = 100% <br/>
        /// 100.000 * 0.001 = 100%
        /// </summary>
        public uint Rate;
    }

    [System.Serializable]
    public class GachaCardModel : GachaRewardModel
    {
        /// <summary>
        /// Danh sách có thể nhận trong đây
        /// </summary>
        public List<uint> CardTemplateIds;

        /// <summary>
        /// số lượng
        /// </summary>
        public uint Quantity;

        public bool IsHaveCard;
    }

    [System.Serializable]
    public class GachaEquipmentModel : GachaRewardModel
    {
        /// <summary>
        /// Danh sách có thể nhận trong đây
        /// </summary>
        public List<uint> EquipmentTemplateIds;
    }
}