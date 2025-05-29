using System;
using World.TheCard;

namespace Popups
{
    /// <summary>
    /// xác định popup này mở lên từ đâu và dựa vào đó để Hiện các chức năng cân có, bỏ các chức năng ko nên có
    /// </summary>
    public abstract class PopupCardModel
    {
        public CardModel CardModel;
        public Action OnClose;
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