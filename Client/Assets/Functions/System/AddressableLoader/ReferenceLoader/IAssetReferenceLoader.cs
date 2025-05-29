using Cysharp.Threading.Tasks;

public interface IAssetReferenceLoader
{
    UniTask Load();
    void Release();
}