using System;
using System.Collections.Generic;
using Functions.World.Player;
using Functions.World.Player.Inventory;
using UniRx;

namespace World.Player.Character
{
    [System.Serializable]
    public class CharacterModel
    {
        public string Id;
        public ushort AvatarId;
        public ushort AvatarFrameId;
        public string Name;
        public bool Gender;
        public DateTime Birthday;
        public string Bio;
        public List<CurrencyModel> Currencies;
        public InventoryModel Inventory;
        /// <summary>
        /// Card player đang sở hữu
        /// </summary>
        public CardCollectionModel CardCollection;

        /// <summary>
        /// Số team có thể setup trước, hiện tối đa 4
        /// </summary>
        public byte MaxLineupTeamCount;
        /// <summary>
        /// có the setup trước lineup cho nhiều đội hình giúp khắc chế tốt hơn, nhưng sẽ tốn tiền để mở thêm slot lineup :)) <br/>
        /// Index bắt đầu từ 0
        /// </summary>
        public List<CardLineupModel> CardLineups;
        
        //===============================[EVENTS]===============================//
        //--------------
        public readonly Subject<Unit> OnCurrencyChanged = new Subject<Unit>();
        public void InvokeOnCurrencyChanged() => OnCurrencyChanged.OnNext(Unit.Default);
        //--------------
        public readonly Subject<Unit> OnCardCollectionChanged = new Subject<Unit>();
        public void InvokeOnCardCollectionChanged() => OnCardCollectionChanged.OnNext(Unit.Default);
        //---------------
        public readonly Subject<Unit> OnCardLineupChanged = new Subject<Unit>();
        public void InvokeOnCardLineupChanged() => OnCardLineupChanged.OnNext(Unit.Default);
        //========================================================================//
        public void SetDefault()
        {
            Id = Guid.NewGuid().ToString();
            AvatarId = 1;
            AvatarFrameId = 1;
            Name = "Player";
            Gender = true;
            Birthday = new DateTime(2001, 10, 25);
            Bio = "";
            Currencies = new List<CurrencyModel>()
            {
                new CurrencyModel()
                {
                    Type = CurrencyType.Circuit,
                    Amount = 0
                },
                new CurrencyModel()
                {
                    Type = CurrencyType.Scrap,
                    Amount = 0
                }
            };
            Inventory = new InventoryModel();
            CardCollection = new CardCollectionModel();
            MaxLineupTeamCount = 3;
            CardLineups = new List<CardLineupModel>();
        }
    }
}