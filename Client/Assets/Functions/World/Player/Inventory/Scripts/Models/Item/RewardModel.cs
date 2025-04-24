using System;
using System.Collections.Generic;
using World.TheCard;
using World.Player.Character;

namespace World.Requirement
{
    [Serializable]
    public class RewardModel
    {
        public List<CurrencyRewardModel> Currencies;
        public List<ExpRewardModel> Exps;
        public List<ItemRewardModel> Items;
    }
    [Serializable]
    public struct CurrencyRewardModel
    {
        public CurrencyType Type;
        public uint MinAmount;
        public uint MaxAmount;
    }
    [Serializable]
    public struct ExpRewardModel
    {
        public LevelType Type;
        public uint MinAmount;
        public uint MaxAmount;
    }
    [Serializable]
    public struct ItemRewardModel
    {
        public ushort ItemTemplateId;
        /// <summary>
        /// Quality need if need | origin = template
        /// </summary>
        public QualityType Quality;
        public ushort MinQuantity;
        public ushort MaxQuantity;
    }
}