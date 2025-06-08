using System.Collections.Generic;
using Functions.World.Data;
using Functions.World.Player;
using Functions.World.Player.Inventory;
using UnityEngine.Serialization;
using World.Player.Character;

namespace Save
{
    [System.Serializable]
    public class SavePlayerModel : SaveModel
    {
        public CharacterModel CharacterModel;

        public SavePlayerModel()
        {
            DataName = "Player";
        }

        public override void SetDefault()
        {
            base.SetDefault();
            CharacterModel = new CharacterModel();
            CharacterModel.SetDefault();
        }
    }

    [System.Serializable]
    public class SaveCurrencyModel : SaveModel
    {
        public List<CurrencyModel> Currencies;

        public override void SetDefault()
        {
            base.SetDefault();
            Currencies = new List<CurrencyModel>()
            {
                new CurrencyModel()
                {
                    Type = CurrencyType.Scrap,
                    Amount = 0
                },
                new CurrencyModel()
                {
                    Type = CurrencyType.Circuit,
                    Amount = 0
                },
            };
        }
    }

    [System.Serializable]
    public class SaveInventoryModel : SaveModel
    {
        public InventoryModel Inventory;
        public override void SetDefault()
        {
            base.SetDefault();
            Inventory = new InventoryModel();
        }
    }

    [System.Serializable]
    public class SaveCardModel : SaveModel
    {
        public CardCollectionModel CardCollection;
        public byte MaxLineupTeamCount;
        public List<CardLineupModel> CardLineups;
        public override void SetDefault()
        {
            base.SetDefault();
            CardCollection = new CardCollectionModel();
            MaxLineupTeamCount = 3;
            CardLineups = new List<CardLineupModel>();
        }
    }

    [System.Serializable]
    public class SaveUniqueIdentityModel : SaveModel
    {
        public UniqueIdentityModel UniqueIdentity;
        public override void SetDefault()
        {
            base.SetDefault();
            UniqueIdentity = new UniqueIdentityModel();
        }
    }
}