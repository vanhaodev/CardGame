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
    }
}