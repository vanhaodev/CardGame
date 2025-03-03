using Utils;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Globals
{
    public class GlobalConfig : SingletonMonoBehavior<GlobalConfig>
    {


        public async UniTask Init()
        {
            // Load TMP_SpriteAsset thay vì SpriteAsset
            // var mainSpriteAsset =
            //     await GlobalFunction.Instance.AddressableLoader.LoadAssetAsync<TMP_SpriteAsset>(
            //         "SpriteAsset/MainSpriteAsset.asset");
            //
            // if (mainSpriteAsset != null)
            // {
            //     TMP_Settings.defaultSpriteAsset = mainSpriteAsset;
            // }
            // else
            // {
            //     Debug.LogError("TMP Sprite Asset not found!");
            // }
        }

    }
}