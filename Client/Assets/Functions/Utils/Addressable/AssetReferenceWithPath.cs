using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Utils
{
    [System.Serializable]
    public class AssetReferenceWithPath<T> where T : UnityEngine.Object
    {
        [SerializeField] private AssetReferenceT<T> _assetRef;

        [SerializeField] private string _addressPath;

        public AssetReferenceT<T> AssetRef => _assetRef;
        public string AddressPath => _addressPath;

#if UNITY_EDITOR
        [Button]
        public void SetPath()
        {
            if (_assetRef == null || string.IsNullOrEmpty(_assetRef.AssetGUID))
                return;

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return;

            var entry = settings.FindAssetEntry(_assetRef.AssetGUID);
            if (entry != null)
            {
                _addressPath = entry.address;
                Debug.Log($"New AddressPath: {_addressPath}");
            }
        }
#endif
    }
    
    //no rule
    [System.Serializable]
    public class AssetReferenceWithPath
    {
        [SerializeField] private AssetReference _assetRef;

        [SerializeField] private string _addressPath;

        public AssetReference AssetRef => _assetRef;
        public string AddressPath => _addressPath;

#if UNITY_EDITOR
        [Button]
        public void SetPath()
        {
            if (_assetRef == null || string.IsNullOrEmpty(_assetRef.AssetGUID))
                return;

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return;

            var entry = settings.FindAssetEntry(_assetRef.AssetGUID);
            if (entry != null)
            {
                _addressPath = entry.address;
                Debug.Log($"New AddressPath: {_addressPath}");
            }
        }
#endif
    }
}