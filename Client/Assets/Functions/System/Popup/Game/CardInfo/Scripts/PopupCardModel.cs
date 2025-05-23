using World.TheCard;

namespace Popups
{
    public abstract class PopupCardModel
    {
        public CardModel CardModel;
    }

    public class PopupCardEquipModel : PopupCardModel
    {
        public byte LineupIndex;
        public byte SlotIndex;
    }

    public class PopupCardUnequipModel : PopupCardModel
    {
        public byte LineupIndex;
        public byte SlotIndex;
    }

    public class PopupCardCollectionModel : PopupCardModel
    {
    }

    public class PopupCardBattleModel : PopupCardModel
    {
    }
}