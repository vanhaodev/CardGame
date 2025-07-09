namespace Functions.World.Player.Inventory
{
    [System.Serializable]
    public class ItemResourceTemplateModel : ItemTemplateModel
    {
        public ItemResourceTemplateModel()
        {
            ItemType = ItemType.Resource;
        }
    }

    [System.Serializable]
    public class ItemCardShardTemplateModel : ItemResourceTemplateModel
    {
        public ushort CardTemplateId;
    }
}