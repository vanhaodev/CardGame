using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Globals;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.Player.PopupCharacter;

namespace GameConfigs
{
    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        public async UniTask InitItem()
        {
        }

        private ConcurrentDictionary<uint, ItemTemplateModel> _itemTemplates = new();

        public async UniTask<ItemTemplateModel> GetItemTemplate(uint templateId)
        {
            if (_itemTemplates.TryGetValue(templateId, out var value))
            {
                return value;
            }

            var itemTemplateSO = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<ItemTemplateSO>("ItemTemplates/" + templateId);
            _itemTemplates.TryAdd(templateId, itemTemplateSO.Model);
            return itemTemplateSO.Model;
        }

        private ConcurrentDictionary<uint, Sprite> _loadedItemTemplateSprites = new();

        public async UniTask<Sprite> GetItemIcon(uint templateId)
        {
            if (_loadedItemTemplateSprites.TryGetValue(templateId, out var value))
            {
                return value;
            }

            var sprite = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<Sprite>("ItemIcons/" + templateId);
            _loadedItemTemplateSprites.TryAdd(templateId, sprite);
            return sprite;
        }

        /// <summary>
        /// Tăng chỉ số theo cấp cường hoá
        /// </summary>
        /// <param name="upgradeLevel"></param>
        /// <returns></returns>
        public float UpgradeItemPercentBonus(byte upgradeLevel)
        {
            switch (upgradeLevel)
            {
                case 0: return 0f;
                case 1: return 1.6f;
                case 2: return 3.2f;
                case 3: return 5.0f;
                case 4: return 7.0f;
                case 5: return 9.5f;
                case 6: return 12.0f;
                case 7: return 15.0f;
                case 8: return 18.5f;
                case 9: return 22.0f;
                case 10: return 26.0f;
                default: return 26.0f;
            }
        }

        /// <summary>
        /// tỉ lệ cường hoá thành công theo cấp
        /// </summary>
        /// <param name="upgradeLevel"></param>
        /// <returns></returns>
        public ushort UpgradeItemSuccessRate(byte upgradeLevel)
        {
            switch (upgradeLevel)
            {
                case 1: return 8500; // 85%
                case 2: return 7000; // 70%
                case 3: return 5500; // 55%
                case 4: return 4000; // 40%
                case 5: return 3000; // 30%
                case 6: return 2200; // 22%
                case 7: return 1600; // 16%
                case 8: return 1200; // 12%
                case 9: return 1100; // 11%
                case 10: return 1000; // 10%
                default: return 0; // Ko hợp lệ hoặc ko nâng được
            }
        }

        public uint UpgradeItemScrapCost(byte upgradeLevel)
        {
            switch (upgradeLevel)
            {
                case 1: return 1000; // 85%
                case 2: return 1000; // 70%
                case 3: return 1000; // 55%
                case 4: return 3000; // 40%
                case 5: return 3000; // 30%
                case 6: return 3000; // 22%
                case 7: return 7000; // 16%
                case 8: return 7000; // 12%
                case 9: return 9000; // 11%
                case 10: return 15000; // 10%
                default: return 0;
            }
        }
    }
}