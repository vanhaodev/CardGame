using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Globals;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.TheCard;
using World.Requirement;

namespace GameConfigs
{
    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        public IReadOnlyDictionary<ItemGroupType, ItemGroupTypeTemplateModel> ItemTypeGroups { get; private set; }
        public IReadOnlyDictionary<ItemType, ItemTypeTemplateModel> ItemTypes { get; private set; }
        private ConcurrentDictionary<ushort, ItemTemplateModel> _itemTemplates = new();

        public async UniTask<ItemTemplateModel> GetItemTemplate(ushort templateId)
        {
            if (_itemTemplates.TryGetValue(templateId, out var value))
            {
                return value;
            }

            var textSO = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<TextSO>("ItemTemplates/" + templateId + ".asset");
            var newCard = JsonConvert.DeserializeObject<ItemTemplateModel>(textSO.Content);
            _itemTemplates.TryAdd(templateId, newCard);
            return newCard;
        }

        private ConcurrentDictionary<ushort, Sprite> _loadedItemTemplateSprites = new();

        public async UniTask<Sprite> GetItemTemplateSprite(ushort templateId)
        {
            if (_loadedItemTemplateSprites.TryGetValue(templateId, out var value))
            {
                return value;
            }

            var sprite = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<Sprite>("ItemTemplates/" + templateId + ".asset");
            _loadedItemTemplateSprites.TryAdd(templateId, sprite);
            return sprite;
        }

        public async UniTask InitItem()
        {
            var paths = new[]
            {
                "ItemTemplates/ItemTypeGroupsSO.asset",
                "ItemTemplates/ItemTypesSO.asset"
            };

            var tasks = new List<UniTask<TextSO>>();
            foreach (var path in paths)
            {
                tasks.Add(Global.Instance.Get<AddressableLoader>().LoadAssetAsync<TextSO>(path).AsUniTask());
            }

            var results = await UniTask.WhenAll(tasks);

            // Deserialize từ JSON List<>
            var itemGroupTypesList =
                JsonConvert.DeserializeObject<List<ItemGroupTypeTemplateModel>>(results[0].Content);
            var itemTypesList = JsonConvert.DeserializeObject<List<ItemTypeTemplateModel>>(results[1].Content);

            // Chuyển List thành Dictionary
            ItemTypeGroups = itemGroupTypesList.ToDictionary(x => x.Type);
            ItemTypes = itemTypesList.ToDictionary(x => x.Type);

            // Debug.Log($"Loaded\n" +
            //           $"{ItemTypeGroups.Count} item groups" +
            //           $"\n{ItemTypes.Count} item types");
        }
    }
}