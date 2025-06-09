using GameConfigs;
using Globals;
using World.Player.Character;
using World.TheCard;
using World.TheCard.Skill;

namespace Functions.World.Player.Inventory
{
    public partial class InventoryModel
    {
        public CardModel AddNewCard(uint cardTemplateId, uint quantity = 1)
        {
            var chaData = Global.Instance.Get<CharacterData>();
            var card = new CardModel
            {
                Id = chaData.UniqueIdentityModel.CardId.GetValue(),
                TemplateId = cardTemplateId,
                Star = 1,
                Level = new CardLevelModel()
            };
            return card;
        }

        public ItemResourceModel AddNewItemResource(uint cardTemplateId, uint quantity = 1)
        {
            
        }
    }
}