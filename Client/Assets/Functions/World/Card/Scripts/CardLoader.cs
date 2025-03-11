using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace World.Card
{
    public class CardLoader : MonoBehaviour, IGlobal
    {
        [SerializeField] private CardTemplateListSO _cardTemplateListSO;
        private Dictionary<ushort, CardTemplateSO> _loadedCards = new(); // Cache đã load

        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<CardTemplateSO> GetCardTemplate(ushort id)
        {
            // Nếu đã load, trả về ngay
            if (_loadedCards.TryGetValue(id, out var card))
            {
                return card;
            }

            // Tiến hành tải card mới
            var cardTemplate = await LoadCardTemplate(id);

            // Nếu tải thành công, lưu vào cache
            if (cardTemplate != null)
            {
                _loadedCards[id] = cardTemplate;
            }

            return cardTemplate;
        }

        private async UniTask<CardTemplateSO> LoadCardTemplate(ushort id)
        {
            if (!_cardTemplateListSO.CardTemplateRefs.TryGetValue(id, out var assetReference))
            {
                Debug.LogError($"❌ Card ID {id} không tồn tại trong Addressables!");
                return null;
            }

            return await EnsureLoaded(assetReference);
        }

        private async UniTask<CardTemplateSO> EnsureLoaded(AssetReferenceT<CardTemplateSO> assetReference)
        {
            if (!assetReference.RuntimeKeyIsValid()) 
            {
                Debug.LogError("❌ AssetReference không hợp lệ!");
                return null;
            }

            // Nếu asset đã được load, trả về ngay
            if (assetReference.OperationHandle.IsValid())
            {
                if (assetReference.OperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    return assetReference.OperationHandle.Result as CardTemplateSO;
                }
                else
                {
                    // Chờ nếu đang load
                    // Debug.LogWarning("⚠️ AssetReference đang trong quá trình load, chờ hoàn tất...");
                    await assetReference.OperationHandle.Task;
                    return assetReference.OperationHandle.Result as CardTemplateSO;
                }
            }

            // Nếu chưa load, tiến hành load
            var handle = assetReference.LoadAssetAsync<CardTemplateSO>();
            await handle.Task;

            return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
        }

        public void ReleaseAllLoadedCards()
        {
            foreach (var pair in _cardTemplateListSO.CardTemplateRefs)
            {
                if (_loadedCards.ContainsKey(pair.Key))
                {
                    pair.Value.ReleaseAsset();
                }
            }
            _loadedCards.Clear();
        }

        private void OnDestroy()
        {
            ReleaseAllLoadedCards();
        }

        // ================== #EDITOR TESTING ==================
#if UNITY_EDITOR
        [SerializeField] private List<CardTemplateSO> _cardList;
        [ContextMenu("Load All Cards (EDITOR)")]
        public async void LoadAllCardsEditor()
        {
            if (_cardTemplateListSO == null)
            {
                Debug.LogError("❌ _cardTemplateListSO chưa được gán trong Inspector!");
                return;
            }

            foreach (var id in _cardTemplateListSO.CardTemplateRefs.Keys)
            {
                _cardList.Add(await GetCardTemplate(id)); // Gọi đúng hệ thống runtime
            }

            Debug.Log("✅ Load tất cả Card hoàn tất!");
        }
#endif
    }
}
