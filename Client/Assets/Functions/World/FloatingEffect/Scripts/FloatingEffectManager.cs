using System.Collections.Generic;
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
        [SerializeField] Transform _instantiateContainer;
        private Dictionary<string, DynamicObjectPool<FloatingEffect>> effectPools =
            new Dictionary<string, DynamicObjectPool<FloatingEffect>>();

        private Dictionary<string, AsyncOperationHandle<GameObject>> loadedPrefabs =
            new Dictionary<string, AsyncOperationHandle<GameObject>>();

        // Hàm thực hiện hiệu ứng bay lên và mờ dần
        private void DoEffect(FloatingEffect effect, Vector2 position)
        {
            if (effect == null) return;

            Transform effectTransform = effect.transform;
            effectTransform.position = position;

            CanvasGroup canvasGroup =
                effect.GetComponent<CanvasGroup>() ?? effect.gameObject.AddComponent<CanvasGroup>();

            float duration = 1.0f;
            float moveDistance = 4f;

            effectTransform.DOMoveY(position.y + moveDistance, duration).SetEase(Ease.OutQuad);
            canvasGroup.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                ReleaseEffect(effect); // Trả về pool sau khi hoàn thành
            });
        }

        // Hàm Spawn hiệu ứng
        public FloatingEffect SpawnFloatingEffect(string prefabAddress, Vector3 position)
        {
            if (effectPools.TryGetValue(prefabAddress, out var pool))
            {
                FloatingEffect effect = pool.Get();
                effect.transform.position = position;
                effect.gameObject.SetActive(true);
                return effect;
            }

            FloatingEffect floatingEffect = null;

            if (!loadedPrefabs.TryGetValue(prefabAddress, out var handle))
            {
                handle = Addressables.LoadAssetAsync<GameObject>(prefabAddress);
                loadedPrefabs[prefabAddress] = handle;
            }

            handle.Completed += loadHandle =>
            {
                if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject prefab = loadHandle.Result;

                    effectPools[prefabAddress] = new DynamicObjectPool<FloatingEffect>(
                        createFunc: () =>
                        {
                            GameObject instance = Instantiate(prefab, _instantiateContainer);
                            var effect = instance.GetComponent<FloatingEffect>();
                            effect.PrefabAddress = prefabAddress; // Lưu prefabAddress vào object
                            return effect;
                        },
                        resetAction: effect => { effect.gameObject.SetActive(false); });

                    FloatingEffect effectInstance = effectPools[prefabAddress].Get();
                    effectInstance.transform.position = position;
                    effectInstance.gameObject.SetActive(true);
                    floatingEffect = effectInstance;
                }
                else
                {
                    Debug.LogError($"Failed to load prefab: {prefabAddress}");
                }
            };

            return floatingEffect;
        }

        // Hàm giải phóng hiệu ứng
        public void ReleaseEffect(FloatingEffect effect)
        {
            if (effect == null) return;

            string prefabAddress = effect.PrefabAddress;

            if (effectPools.TryGetValue(prefabAddress, out var pool))
            {
                pool.Put(effect);
            }
            else
            {
                Destroy(effect.gameObject); // Hủy hoàn toàn nếu không thuộc pool
            }
        }

        // Hàm giải phóng tất cả tài nguyên
        public void ReleaseAll()
        {
            foreach (var kvp in loadedPrefabs)
            {
                Addressables.Release(kvp.Value); // Giải phóng handle đúng cách
            }

            loadedPrefabs.Clear();

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
            Debug.Log("Released all pooled effects and unloaded Addressables assets.");
        }

        private void OnDisable()
        {
            ReleaseAll();
        }
    }

    public partial class FloatingEffectManager : MonoBehaviour
    {
        // Hàm hiển thị Damage
        [Button]
        public void ShowDamage(string value, Vector3 position)
        {
            string prefabAddress = "FloatingEffect/Prefabs/FloatingEffect.prefab"; // Địa chỉ prefab cho Damage
            FloatingEffect effect = SpawnFloatingEffect(prefabAddress, position);
            // Set giá trị text hoặc các thông số khác của effect tại đây (nếu cần)
            effect.SetText(value);
            DoEffect(effect, position); // Chạy hiệu ứng
        }

        // Hàm hiển thị Hp Healing
        public void ShowHpHealing(string value, Vector3 position)
        {
            string prefabAddress = "HpHealingPrefabAddress"; // Địa chỉ prefab cho Healing
            FloatingEffect effect = SpawnFloatingEffect(prefabAddress, position);
            effect.GetComponentInChildren<TextMesh>().text = value;
            DoEffect(effect, position);
        }

        // Hàm hiển thị Mp Healing
        public void ShowMpHealing(string value, Vector3 position)
        {
            string prefabAddress = "MpHealingPrefabAddress"; // Địa chỉ prefab cho Healing
            FloatingEffect effect = SpawnFloatingEffect(prefabAddress, position);
            effect.GetComponentInChildren<TextMesh>().text = value;
            DoEffect(effect, position);
        }

        // Hàm hiển thị Crit
        public void ShowCrit(Vector3 position)
        {
            string prefabAddress = "CritPrefabAddress"; // Địa chỉ prefab cho Crit
            FloatingEffect effect = SpawnFloatingEffect(prefabAddress, position);
            // Set giá trị text hoặc các thông số khác của effect tại đây (nếu cần)
            effect.GetComponentInChildren<TextMesh>().text = "CRIT!";
            DoEffect(effect, position);
        }

        // Hàm hiển thị Dodge
        public void ShowDodge(Vector3 position)
        {
            string prefabAddress = "DodgePrefabAddress"; // Địa chỉ prefab cho Dodge
            FloatingEffect effect = SpawnFloatingEffect(prefabAddress, position);
            // Set giá trị text hoặc các thông số khác của effect tại đây (nếu cần)
            effect.GetComponentInChildren<TextMesh>().text = "DODGE!";
            DoEffect(effect, position);
        }
    }
}