using System;
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
            chaData.CharacterModel.CardCollection.Cards.Add(card);
            return card;
        }

        public ItemResourceModel AddNewCardShard(uint itemTemplateId, uint quantity = 1)
        {
            var itemInInv = GetItemByTemplateId(itemTemplateId);
            if (itemInInv != null)
            {
                itemInInv.Quantity += quantity;
                return itemInInv.Item as ItemResourceModel;
            }
            else
            {
                var chaData = Global.Instance.Get<CharacterData>();
                var item = new ItemResourceModel()
                {
                    Id = chaData.UniqueIdentityModel.ItemId.GetValue(),
                    TemplateId = itemTemplateId,
                    Rarity = ItemRarity.UR,
                    CreatedAt = DateTime.UtcNow
                };
                Items.Add(new InventoryItemModel()
                {
                    Item = item,
                    Quantity = quantity
                });
                return item;
            }
        }
    }
}