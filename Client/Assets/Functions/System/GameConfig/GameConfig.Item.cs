using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Functions.World.Gacha;
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
        public ushort RequiredShardCount = 100;

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
                case 1: return 3.6f;
                case 2: return 5.2f;
                case 3: return 7.0f;
                case 4: return 9.0f;
                case 5: return 12.5f;
                case 6: return 15.0f;
                case 7: return 22.0f;
                case 8: return 27.5f;
                case 9: return 32.0f;
                case 10: return 37.0f;
                default: return 37.0f;
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
                case 3: return 7000; // 70%
                case 4: return 7000; // 70%
                case 5: return 5500; // 55%
                case 6: return 5500; // 55%
                case 7: return 4000; // 40%
                case 8: return 3000; // 30%
                case 9: return 2400; // 24%
                case 10: return 2000; // 20%
                default: return 0; // Ko hợp lệ hoặc ko nâng được
            }
        }

        public uint UpgradeItemScrapCost(byte upgradeLevel)
        {
            switch (upgradeLevel)
            {
                case 1: return 1000;
                case 2: return 1000;
                case 3: return 1000;
                case 4: return 3000;
                case 5: return 3000;
                case 6: return 3000;
                case 7: return 7000;
                case 8: return 7000;
                case 9: return 9000;
                case 10: return 15000;
                default: return 0;
            }
        }
    }

    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        [SerializeReference] private List<GachaEquipmentModel> _gachaEquipment1; //gacha sắt
        [SerializeReference] private List<GachaEquipmentModel> _gachaEquipment2; //gacha bạc
        [SerializeReference] private List<GachaEquipmentModel> _gachaEquipment3; //gacha vàng

        /// <summary>
        /// 1: gỗ <br/>
        /// 2: bạc <br/>
        /// 3: vàng <br/>
        /// </summary>
        /// <param name="type"></param>
        public async UniTask<List<GachaEquipmentModel>> GetEquipmentGacha(int type)
        {
            switch (type)
            {
                case 1:
                {
                    if (_gachaEquipment1 == null || _gachaEquipment1.Count == 0)
                    {
                        var json1 = await Global.Instance.Get<AddressableLoader>()
                            .LoadAssetAsync<TextSO>("CardConfigs/GachaEquipment1");
                        _gachaEquipment1 = JsonConvert.DeserializeObject<List<GachaEquipmentModel>>(json1.Content);
                    }

                    return _gachaEquipment1;
                }
                case 2:
                {
                    if (_gachaEquipment2 == null || _gachaEquipment2.Count == 0)
                    {
                        var json2 = await Global.Instance.Get<AddressableLoader>()
                            .LoadAssetAsync<TextSO>("CardConfigs/GachaEquipment2");
                        _gachaEquipment2 = JsonConvert.DeserializeObject<List<GachaEquipmentModel>>(json2.Content);
                    }

                    return _gachaEquipment2;
                }
                case 3:
                {
                    if (_gachaEquipment3 == null || _gachaEquipment3.Count == 0)
                    {
                        var json3 = await Global.Instance.Get<AddressableLoader>()
                            .LoadAssetAsync<TextSO>("CardConfigs/GachaEquipment3");
                        _gachaEquipment3 = JsonConvert.DeserializeObject<List<GachaEquipmentModel>>(json3.Content);
                    }

                    return _gachaEquipment3;
                }
            }

            return null;
        }
    }
}