using System;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using World.TheCard;
using World.Player.Character;

namespace World.Requirement
{
    [Serializable]
    public class RequirementModel
    {
        //json
        /*
         {
  "Jobs": [
    "JobType1",
    "JobType2"
  ],
  "Levels": [
    {
      "Type": "LevelType1",
      "Min": 1,
      "Max": 10
    },
    {
      "Type": "LevelType2",
      "Min": 5,
      "Max": 20
    }
  ],
  "Currency": {
    "CurrencyType": "CurrencyType1",
    "Amount": 100
  },
  "Items": [
    {
      "ItemTemplateId": 1,
      "Quality": "Good",
      "Quantity": 5
    },
    {
      "ItemTemplateId": 2,
      "Quality": "Rare",
      "Quantity": 3
    }
  ],
  "CompletedQuestIds": [
    101,
    102
  ]
}

         */
        // public List<JobType> Jobs;
        public List<LevelRequirementModel> Levels;
        public List<AttributeRequirementModel> Attributes;
        /// <summary>
        /// sẽ bị lấy đi khi nếu là hành động trao đổi (học, nâng cấp...)
        /// </summary>
        public List<CurrencyModel> Currencies;
        /// <summary>
        /// sẽ bị lấy đi khi nếu là hành động trao đổi (học, nâng cấp...)
        /// </summary>
        public List<ItemRequirementModel> Items;
    }
    [Serializable]
    public struct AttributeRequirementModel
    {
      public AttributeType Type;
      public ushort Min;
      public ushort Max;
    }
    [Serializable]
    public struct LevelRequirementModel
    {
        public LevelType Type;
        public ushort Min;
        public ushort Max;
    }

    [Serializable]
    public struct ItemRequirementModel
    {
        public ushort ItemTemplateId;

        /// <summary>
        /// Quality need if need | none = any
        /// </summary>
        public QualityType Quality;

        public ushort Quantity;
    }
}