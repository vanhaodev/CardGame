using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;

namespace FloatingEffect
{
    public partial class FloatingEffectManager : MonoBehaviour
    {
        // Hàm hiển thị Damage
        [Button]
        public async void ShowDamage(int value, Vector3 position)
        {
            string prefabAddress = "FloatingEffect/FloatingEffectDamage.prefab"; // Địa chỉ prefab cho Damage

            // Lấy hiệu ứng từ pool hoặc tạo mới nếu không có
            FloatingEffect effect = await GetEffectFromPoolOrCreate(prefabAddress);

            // Set giá trị text hoặc các thông số khác của effect tại đây (nếu cần)
            effect.SetText($"-{value.ToString("N0")}");

            // Lấy Transform của hiệu ứng để thay đổi vị trí
            Transform effectTransform = effect.transform;
            float bonusYPos = Random.Range(-0.4f, 0.4f);
            effectTransform.position = new Vector3(position.x, position.y + bonusYPos, position.z);

            // Đảm bảo rằng hiệu ứng hiển thị với alpha = 1 (hiển thị đầy đủ)
            effect.GetCanvasGroup().alpha = 1;

            // Cài đặt thời gian hiệu ứng kéo dài và các giá trị ngẫu nhiên
            float duration = 1.0f; // Thời gian hiệu ứng di chuyển
            float jumpPower = 0.75f; // Độ cao của cú nhảy
            float randomX = Random.Range(-0.5f, 0.5f); // Giá trị ngẫu nhiên cho sự dịch chuyển theo X (trái hoặc phải)

            // Sử dụng DOJump để tạo hiệu ứng nhảy với chuyển động ngẫu nhiên sang trái hoặc phải
            Vector3 randomPosition =
                new Vector3(position.x + randomX, position.y + bonusYPos, position.z); // Tính toán vị trí ngẫu nhiên mới
            await effectTransform.DOJump(randomPosition, jumpPower, 1, duration)
                .AsyncWaitForCompletion(); // Chờ đến khi hiệu ứng hoàn tất

            // Sau khi hiệu ứng kết thúc, làm mờ đi (fade out) hiệu ứng để làm nó biến mất
            effect.GetCanvasGroup().DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                // Trả hiệu ứng về pool sau khi kết thúc để tái sử dụng
                ReleaseEffect(effect); // Nếu bạn sử dụng hệ thống pool để quản lý lại đối tượng này
            });
        }
    }

    public partial class FloatingEffectManager : MonoBehaviour
    {
        [SerializeField] Transform _instantiateContainer;

        private Dictionary<string, DynamicObjectPool<FloatingEffect>> effectPools =
            new Dictionary<string, DynamicObjectPool<FloatingEffect>>();

        // Hàm lấy hiệu ứng từ pool hoặc tạo mới nếu chưa có
        private async UniTask<FloatingEffect> GetEffectFromPoolOrCreate(string prefabAddress)
        {
            // Kiểm tra nếu hiệu ứng đã được tải và có trong pool rồi
            if (effectPools.TryGetValue(prefabAddress, out var pool))
            {
                FloatingEffect effect = pool.Get(); // Lấy hiệu ứng từ pool
                if (effect != null)
                {
                    effect.gameObject.SetActive(true);
                    return effect;
                }
            }

            // Nếu chưa có trong pool hoặc hiệu ứng đã bị sử dụng hết, tiến hành tải prefab từ Addressables
            GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(prefabAddress).ToUniTask();

            // Kiểm tra nếu prefab được tải thành công
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab: {prefabAddress}");
                return null;
            }

            // Nếu chưa có pool cho prefab, tạo mới pool
            if (!effectPools.ContainsKey(prefabAddress))
            {
                effectPools[prefabAddress] = new DynamicObjectPool<FloatingEffect>(
                    createFunc: () =>
                    {
                        GameObject instance = Instantiate(prefab, _instantiateContainer);
                        var effect = instance.GetComponent<FloatingEffect>();
                        effect.PrefabAddress = prefabAddress; // Lưu prefabAddress vào object
                        return effect;
                    },
                    resetAction: effect => { effect.gameObject.SetActive(false); });
            }

            // Lấy hiệu ứng từ pool và trả về
            FloatingEffect effectInstance = effectPools[prefabAddress].Get();
            effectInstance.gameObject.SetActive(true);

            return effectInstance;
        }

        // Hàm giải phóng hiệu ứng
        public void ReleaseEffect(FloatingEffect effect)
        {
            if (effect == null) return;

            string prefabAddress = effect.PrefabAddress;

            if (effectPools.TryGetValue(prefabAddress, out var pool))
            {
                pool.Put(effect); // Trả về pool khi không sử dụng nữa
            }
            else
            {
                Destroy(effect.gameObject); // Hủy hoàn toàn nếu không thuộc pool
            }
        }

        // Hàm giải phóng tất cả tài nguyên
        public void ReleaseAll()
        {
            foreach (var pool in effectPools.Values)
            {
                while (pool.Count > 0)
                {
                    FloatingEffect effect = pool.Get();
                    if (effect != null)
                    {
                        Destroy(effect.gameObject); // Hủy GameObject thực sự
                    }
                }

                pool.Clear();
            }

            effectPools.Clear();
            Debug.Log("Released all pooled effects.");
        }

        private void OnDisable()
        {
            ReleaseAll();
        }
    }
}