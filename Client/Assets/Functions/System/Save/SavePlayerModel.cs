using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public override async UniTask SetDefault()
        {
            await  base.SetDefault();
            CharacterModel = new CharacterModel();
            await CharacterModel.SetDefault();
        }
    }

    [System.Serializable]
    public class SaveCurrencyModel : SaveModel
    {
        public List<CurrencyModel> Currencies;

        public override async UniTask SetDefault()
        {
            await  base.SetDefault();
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
        public override async UniTask SetDefault()
        {
            await base.SetDefault();
            Inventory = new InventoryModel();
        }
    }

    [System.Serializable]
    public class SaveCardModel : SaveModel
    {
        public CardCollectionModel CardCollection;
        public byte MaxLineupTeamCount;
        public List<CardLineupModel> CardLineups;
        public override async UniTask SetDefault()
        {
            await base.SetDefault();
            CardCollection = new CardCollectionModel();
            MaxLineupTeamCount = 3;
            CardLineups = new List<CardLineupModel>();
        }
    }

    [System.Serializable]
    public class SaveUniqueIdentityModel : SaveModel
    {
        public UniqueIdentityModel UniqueIdentity;
        public override async UniTask SetDefault()
        {
            await base.SetDefault();
            UniqueIdentity = new UniqueIdentityModel();
        }
    }
}