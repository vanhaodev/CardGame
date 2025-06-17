using Cysharp.Threading.Tasks;
using Functions.World.Gacha;
using UnityEngine;
using Utils;

public class GachaEquipmentResultManager : GachaCardResultManager
{
    [SerializeField] private AssetReferenceWithPath _assetRefSpriteIronChest;
    [SerializeField] private AssetReferenceWithPath _assetRefSpriteSilverChest;
    [SerializeField] private AssetReferenceWithPath _assetRefSpriteGoldenChest;

    public async UniTask SetupChestSprite(byte type)
    {
        Sprite x = null;

        switch (type)
        {
            case 1:
                if (!_assetRefSpriteIronChest.AssetRef.OperationHandle.IsValid())
                    await _assetRefSpriteIronChest.AssetRef.LoadAssetAsync<Sprite>();
                x = _assetRefSpriteIronChest.AssetRef.Asset as Sprite;
                break;

            case 2:
                if (!_assetRefSpriteSilverChest.AssetRef.OperationHandle.IsValid())
                    await _assetRefSpriteSilverChest.AssetRef.LoadAssetAsync<Sprite>();
                x = _assetRefSpriteSilverChest.AssetRef.Asset as Sprite;
                break;

            case 3:
                if (!_assetRefSpriteGoldenChest.AssetRef.OperationHandle.IsValid())
                    await _assetRefSpriteGoldenChest.AssetRef.LoadAssetAsync<Sprite>();
                x = _assetRefSpriteGoldenChest.AssetRef.Asset as Sprite;
                break;
        }

        foreach (EquipmentGacha g in _cardGachas)
        {
            g.SetChestSprite(x);
        }
    }

    public override void Clear()
    {
        base.Clear();

        foreach (EquipmentGacha g in _cardGachas)
        {
            g.SetChestSprite(null); // 👍 xóa sprite khỏi UI
        }

        // 🧹 Giải phóng sprite đã load nếu có
        if (_assetRefSpriteIronChest.AssetRef.OperationHandle.IsValid())
            _assetRefSpriteIronChest.AssetRef.ReleaseAsset();

        if (_assetRefSpriteSilverChest.AssetRef.OperationHandle.IsValid())
            _assetRefSpriteSilverChest.AssetRef.ReleaseAsset();

        if (_assetRefSpriteGoldenChest.AssetRef.OperationHandle.IsValid())
            _assetRefSpriteGoldenChest.AssetRef.ReleaseAsset();
    }
}