namespace Functions.World.Data
{
    /// <summary>
    /// UniqueId
    /// </summary>
    [System.Serializable]
    public class UniqueIdentityModel
    {
        public UniqueIdentityIntModel ItemId;
        public UniqueIdentityIntModel CardId;

        public void SetDefault()
        {
            ItemId = new UniqueIdentityIntModel();
            CardId = new UniqueIdentityIntModel();
            ItemId.SetDefault();
            CardId.SetDefault();
        }
    }

    [System.Serializable]
    public class UniqueIdentityIntModel
    {
        public int Value;

        public int GetValue()
        {
            Value += 1;
            return Value;
        }

        public void SetDefault()
        {
            Value = 0;
        }
    }
}