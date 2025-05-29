using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class SingleAssetReferenceLoader<T> : IAssetReferenceLoader where T : Object
{
    private AsyncOperationHandle<T> _handle;
    private AssetReferenceT<T> _reference;
    private T _loadedAsset;
    private bool _isLoaded = false; // Cờ kiểm tra trạng thái tải tài nguyên

    public SingleAssetReferenceLoader(AssetReferenceT<T> reference)
    {
        _reference = reference;
    }

    public async UniTask Load()
    {
        if (_isLoaded) return; // Nếu đã tải rồi, không tải lại

        if (_reference == null) return;

        _handle = _reference.LoadAssetAsync<T>();
        await _handle.ToUniTask();

        if (_handle.Status == AsyncOperationStatus.Succeeded)
        {
            _loadedAsset = _handle.Result;
            _isLoaded = true; // Đánh dấu là đã tải
        }
    }

    public void Release()
    {
        if (!_isLoaded) return; // Không giải phóng nếu chưa tải tài nguyên

        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
            _loadedAsset = null;
            _isLoaded = false; // Đánh dấu là tài nguyên đã được giải phóng
        }
    }

    public T LoadedAsset => _loadedAsset;
}