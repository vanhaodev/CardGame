using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using World.Card;
using World.Player.Inventory;

namespace GameConfig
{
    public class GameConfig : MonoBehaviour, IGlobal
    {
        public Dictionary<ItemGroupType, ItemGroupTypeTemplateModel> ItemTypeGroups;
        public Dictionary<ItemType, ItemTypeTemplateModel> ItemTypes;
        public Dictionary<ushort, ItemTemplateModel> ItemTemplates;
        public Dictionary<ushort /*level*/, uint /*exp*/> LevelExps;

        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }
}