using System;
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
        public override void SetDefault()
        {
            base.SetDefault();
            IsFirstPlay = true;
        }
    }
}