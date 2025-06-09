namespace Functions.World.Data
{
    /// <summary>
    /// UniqueId
    /// </summary>
    [System.Serializable]
    public class UniqueIdentityModel
    {
        public UniqueIdentityUIntModel ItemId;
        public UniqueIdentityUIntModel CardId;

        public void SetDefault()
        {
            ItemId = new UniqueIdentityUIntModel();
            CardId = new UniqueIdentityUIntModel();
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
    [System.Serializable]
    public class UniqueIdentityUIntModel
    {
        public uint Value;

        public uint GetValue()
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