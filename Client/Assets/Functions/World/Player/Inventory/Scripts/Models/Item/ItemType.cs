namespace World.Requirement
{
    public enum ItemType
    {
        //Consumption (template id from 1)
        Food = 1, //
        Potion, 

        //Resource (template id from 10000)
        UpgradeStone, //Increase success rate
        UpgradeScroll, //Need when upgrade
        UpgradeSwitchScoll, //Switch upgrade level of two items
        Sandalwood, // Gỗ sưa
        CommonWood, // Gỗ thường
        HardLeather, // Da cứng
        SoftLeather, // Da mềm
        Silk, // Tơ lụa
        Fabric, // Vải
        Glass, // Thủy tinh
        Jade, // Ngọc
        Iron, // Sắt
        Silver, // Bạc
        FiveElementStone, // Đá ngũ hợp
        PureFiveElementStone, // Đá ngũ hợp tinh khiết

        //EventCall (template id from 20000)
        TreasureChest,
        CommandScroll,

        //Weapom & Equipment (template id from 30000)
        Sword, Bow, Staff, HeavySword, Hammer,
        Hair,
        Eyes,
        Shirt,
        Pants,
        Gloves,
        Shoes,
        Ring,
        Necklace,
        JadePendant, //Ngọc bội
        Mount,
        Companion,

        //Tool (template id from 40000)
        Hatchet,
        Pickaxe,
        Sickle //Liềm
    }
}