using System;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using World.Player.Character;
using World.TheCard;
using World.TheCard.Skill;

namespace Functions.World.Player.Inventory
{
    public partial class InventoryModel
    {
        public async UniTask <CardModel>AddNewCard(uint cardTemplateId, uint quantity = 1)
        {
            var chaData = Global.Instance.Get<CharacterData>();
            var card = new CardModel
            {
                Id = await chaData.UniqueIdentityModel.CardId.GetValue(),
                TemplateId = cardTemplateId,
                Star = 0,
                Level = new CardLevelModel()
            };
            chaData.CharacterModel.CardCollection.Cards.Add(card);
            card?.UpdateAttribute();
            return card;
        }

        public async UniTask<ItemResourceModel> AddNewCardShard(uint itemTemplateId, uint quantity = 1)
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
                    Id =  await chaData.UniqueIdentityModel.ItemId.GetValue(),
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
        public async UniTask<ItemEquipmentModel> AddNewEquipment(uint itemTemplateId, ItemRarity itemRarity)
        {
            var chaData = Global.Instance.Get<CharacterData>();
            var item = new ItemEquipmentModel()
            {
                Id = await chaData.UniqueIdentityModel.ItemId.GetValue(),
                TemplateId = itemTemplateId,
                Rarity = itemRarity,
                CreatedAt = DateTime.UtcNow
            };
            Items.Add(new InventoryItemModel()
            {
                Item = item,
                Quantity = 1
            });
            item?.UpdateAttribute();
            return item;
        }

        public void RemoveItem(uint itemId, uint quantity = 1)
        {
            var inInv = Items.Find(i => i.Item.Id == itemId);
            if (inInv != null)
            {
                // Nếu quantity <=0 thì xoá luôn
                if (quantity <= 0)
                {
                    Items.Remove(inInv);
                    return;
                }

                // Trừ số lượng
                inInv.Quantity -= (uint)quantity;

                // Nếu sau khi trừ <=0 thì xoá
                if (inInv.Quantity <= 0)
                {
                    Items.Remove(inInv);
                }
            }
        }
    }
}