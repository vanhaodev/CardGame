using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
[System.Serializable]
public class ListAssetReferenceLoader<T> : IAssetReferenceLoader where T : Object
{
    List<AsyncOperationHandle<T>> _handles = new List<AsyncOperationHandle<T>>();
    [SerializeField] private List<AssetReferenceT<T>> _references;
    [SerializeField] List<T> _loadedAssets = new List<T>();
    [SerializeField] bool _isLoaded = false; // Cờ kiểm tra trạng thái tải tài nguyên

    public ListAssetReferenceLoader()
    {
    }

    public ListAssetReferenceLoader(List<AssetReferenceT<T>> references)
    {
        _references = references;
    }

    public async UniTask Load()
    {
        if (_isLoaded) return; // Nếu đã tải rồi, không tải lại

        var tasks = new List<UniTask>();

        foreach (var reference in _references)
        {
            if (reference == null) continue;

            var handle = reference.LoadAssetAsync<T>();
            _handles.Add(handle);

            var task = handle.ToUniTask().ContinueWith(_ =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _loadedAssets.Add(handle.Result);
                }
            });

            tasks.Add(task);
        }

        await UniTask.WhenAll(tasks);
        _isLoaded = true; // Đánh dấu là đã tải
    }

    public void Release()
    {
        if (!_isLoaded) return; // Không giải phóng nếu chưa tải tài nguyên

        foreach (var handle in _handles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        _handles.Clear();
        _loadedAssets.Clear();
        _isLoaded = false; // Đánh dấu là tài nguyên đã được giải phóng
    }

    public IReadOnlyList<T> LoadedAssets => _loadedAssets;
}