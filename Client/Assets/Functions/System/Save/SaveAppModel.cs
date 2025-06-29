using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Save
{
    [Serializable]
    public class SaveAppModel: SaveModel
    {
        public bool IsFirstPlay;

        public SaveAppModel()
        {
            DataName = "App";
        }
        public override async UniTask SetDefault()
        {
            await base.SetDefault();
            IsFirstPlay = true;
        }
    }
}