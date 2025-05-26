using System;

namespace Save
{
    [Serializable]
    public class SaveAppModel: SaveModel
    {
        public bool IsNewbie;
        public override void SetDefault()
        {
            base.SetDefault();
            IsNewbie = true;
        }
    }
}