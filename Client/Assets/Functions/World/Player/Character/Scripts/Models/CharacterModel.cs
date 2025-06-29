using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Functions.World.Player;
using Functions.World.Player.Inventory;
using Newtonsoft.Json;
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
        [Newtonsoft.Json.JsonIgnore] public List<CurrencyModel> Currencies;
        [Newtonsoft.Json.JsonIgnore] public InventoryModel Inventory;

        /// <summary>
        /// Card player đang sở hữu
        /// </summary>
        [Newtonsoft.Json.JsonIgnore] public CardCollectionModel CardCollection;

        /// <summary>
        /// Số team có thể setup trước, hiện tối đa 4
        /// </summary>
        [Newtonsoft.Json.JsonIgnore] public byte MaxLineupTeamCount;

        /// <summary>
        /// có the setup trước lineup cho nhiều đội hình giúp khắc chế tốt hơn, nhưng sẽ tốn tiền để mở thêm slot lineup :)) <br/>
        /// Index bắt đầu từ 0
        /// </summary>
        [Newtonsoft.Json.JsonIgnore] public List<CardLineupModel> CardLineups;

        //===============================[EVENTS]===============================//
        //--------------
        [Newtonsoft.Json.JsonIgnore] public readonly Subject<Unit> OnCurrencyChanged = new Subject<Unit>();

        public void InvokeOnCurrencyChanged() => OnCurrencyChanged.OnNext(Unit.Default);

        //--------------
        [Newtonsoft.Json.JsonIgnore] public readonly Subject<Unit> OnCardCollectionChanged = new Subject<Unit>();

        public void InvokeOnCardCollectionChanged() => OnCardCollectionChanged.OnNext(Unit.Default);

        //---------------
        [Newtonsoft.Json.JsonIgnore] public readonly Subject<Unit> OnCardLineupChanged = new Subject<Unit>();

        public void InvokeOnCardLineupChanged() => OnCardLineupChanged.OnNext(Unit.Default);

        //========================================================================//
        public async UniTask SetDefault()
        {
            Id = Guid.NewGuid().ToString();
            AvatarId = 1;
            AvatarFrameId = 1;
            Name = "Ahyeon";
            Gender = 1 + 1 == 3;
            Birthday = new DateTime(2001, 10, 25);
            Bio = "";
            Currencies = new List<CurrencyModel>()
            {
                new CurrencyModel()
                {
                    Type = CurrencyType.Scrap,
                    Amount = 1000000
                },
                new CurrencyModel()
                {
                    Type = CurrencyType.Circuit,
                    Amount = 10000
                },
            };
            Inventory = new InventoryModel();
            await UniTask.WhenAll(
                Inventory.AddNewNormalItem(3, 5000), 
                Inventory.AddNewNormalItem(6, 5000)
            );
            CardCollection = new CardCollectionModel();
            MaxLineupTeamCount = 3;
            CardLineups = new List<CardLineupModel>();
        }
    }
}