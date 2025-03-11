using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader : MonoBehaviour, IGlobal
{
    private Dictionary<int, AsyncOperationHandle> _loadedAssets = new Dictionary<int, AsyncOperationHandle>();

    /// <summary>
    /// Tải trước tất cả các asset của một label từ remote nếu cần, với callback tiến độ.
    /// </summary>
    public async Task DownloadDependenciesWithProgressAsync(string label, Action<float> onProgress)
    {
        try
        {
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label);

            while (!downloadHandle.IsDone)
            {
                onProgress?.Invoke(downloadHandle.PercentComplete);
                await Task.Yield();
            }

            if (downloadHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to download dependencies for label: {label}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Load một asset đơn lẻ theo tên và trả về kiểu T, hỗ trợ tải từ remote nếu cần.
    /// </summary>
    public async Task<T> LoadAssetAsync<T>(string assetName) where T : class
    {
        try
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetName);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Lưu handle theo instance ID của asset
                int instanceId = handle.Result.GetHashCode();
                _loadedAssets[instanceId] = handle;
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load addressable asset with name: {assetName}");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    /// <summary>
    /// Load tất cả các asset theo label và trả về danh sách kiểu T.
    /// </summary>
    public async Task<List<T>> LoadAllAssetsAsync<T>(string label) where T : class
    {
        List<T> loadedAssetsList = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, asset =>
        {
            // Thêm asset vào danh sách tạm
            loadedAssetsList.Add(asset);
        });

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Lưu từng handle riêng biệt vào dictionary
            foreach (var asset in loadedAssetsList)
            {
                int instanceId = asset.GetHashCode();
                if (!_loadedAssets.ContainsKey(instanceId))
                {
                    // Lưu từng handle riêng biệt
                    _loadedAssets[instanceId] = Addressables.LoadAssetAsync<T>(label);
                }
            }

            return loadedAssetsList;
        }
        else
        {
            Debug.LogError($"Failed to load addressable assets with label: {label}");
            return null;
        }
    }

    /// <summary>
    /// Giải phóng một asset đã load bằng cách truyền ref của asset.
    /// </summary>
    public void Release<T>(T asset) where T : class
    {
        if (asset == null) return;

        int instanceId = asset.GetHashCode();
        if (_loadedAssets.TryGetValue(instanceId, out var handle))
        {
            Addressables.Release(handle);
            _loadedAssets.Remove(instanceId);
        }
        else
        {
            Debug.LogWarning($"Asset not found in loaded assets: {asset}");
        }
    }

    /// <summary>
    /// Release all unused
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var handle in _loadedAssets.Values)
        {
            Addressables.Release(handle);
        }
        _loadedAssets.Clear();
    }

    public UniTask Init()
    {
        return UniTask.CompletedTask;
    }
}
