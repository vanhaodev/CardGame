namespace Functions.World.Player.Inventory
{
    public class ItemResourceTemplateModel : ItemTemplateModel
    {
        public ItemResourceTemplateModel()
        {
            ItemType = ItemType.Resource;
        }
    }
    public class ItemCardShardTemplateModel : ItemResourceTemplateModel
    {
        public ushort CardTemplateId;
        public ushort RequiredShardCount;
    }
    public class ItemCardLevelBoosterTemplateModel : ItemResourceTemplateModel
    {
        public int Exp;
    }
}