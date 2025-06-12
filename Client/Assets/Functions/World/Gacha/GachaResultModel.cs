using System.Collections.Generic;
using Functions.World.Player.Inventory;
using World.TheCard;

namespace Functions.World.Gacha
{
    [System.Serializable]
    public class GachaResultModel
    {
        /// <summary>
        /// 100k = 100% <br/>
        /// 100.000 * 0.001 = 100%
        /// </summary>
        public uint Rate;
    }

    [System.Serializable]
    public class GachaCardModel : GachaResultModel
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

    public class GachaCardRewardModel
    {
        public CardModel Card;
        public ItemModel ShardModel;
        public uint Quantity;
    }
    [System.Serializable]
    public class GachaEquipmentModel : GachaResultModel
    {
        /// <summary>
        /// Danh sách có thể nhận trong đây
        /// </summary>
        public List<uint> EquipmentTemplateIds;
    }
}