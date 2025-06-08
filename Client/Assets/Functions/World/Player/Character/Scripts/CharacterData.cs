using System;
using Cysharp.Threading.Tasks;
using Functions.World.Data;
using Globals;
using Save;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using World.Player.Character;

namespace World.Player.Character
{
    public class CharacterData : MonoBehaviour, IGlobal
    {
        [SerializeField] private CharacterModel _characterModel;
        [SerializeField] private UniqueIdentityModel _uniqueIdentityModel;

        //=======================[EVENTS]===========================\\
        public readonly Subject<Unit> OnCharacterChanged = new Subject<Unit>();

        public void InvokeOnCharacterChanged() => OnCharacterChanged.OnNext(Unit.Default);

        //==========================================================//
        public CharacterModel CharacterModel
        {
            get => _characterModel;
            private set
            {
                _characterModel = value;
                InvokeOnCharacterChanged();
            }
        }

        public UniqueIdentityModel UniqueIdentityModel
        {
            get => _uniqueIdentityModel;
            private set => _uniqueIdentityModel = value;
        }

        public async UniTask Init()
        {
            // Debug.Log("Loading playerData data");
            var save = new SaveManager();
            var app = await save.Load<SaveAppModel>();

            //new player
            if (app.IsFirstPlay)
            {
                app.IsFirstPlay = false;
                CharacterModel = new CharacterModel();
                CharacterModel.SetDefault();
                UniqueIdentityModel = new UniqueIdentityModel();

                await save.Save(app);
                await save.Save(new SavePlayerModel()
                {
                    CharacterModel = CharacterModel
                });
            }
            //old
            else
            {
                var playerData = await save.Load<SavePlayerModel>();
                var currencyData = await save.Load<SaveCurrencyModel>();
                var inventoryData = await save.Load<SaveInventoryModel>();
                var cardData = await save.Load<SaveCardModel>();
                var uniqueIdentityData = await save.Load<SaveUniqueIdentityModel>();
                CharacterModel = playerData.CharacterModel;
                CharacterModel.Currencies = currencyData.Currencies;
                CharacterModel.Inventory = inventoryData.Inventory;
                CharacterModel.CardCollection = cardData.CardCollection;
                CharacterModel.MaxLineupTeamCount = cardData.MaxLineupTeamCount;
                CharacterModel.CardLineups = cardData.CardLineups;
                UniqueIdentityModel = uniqueIdentityData.UniqueIdentity;
            }
        }

        [Button]
        public async void Save()
        {
            await new SaveManager().Save(new SavePlayerModel()
            {
                CharacterModel = CharacterModel,
            });
            await new SaveManager().Save(new SaveCurrencyModel()
            {
                Currencies = CharacterModel.Currencies,
            });
            await new SaveManager().Save(new SaveInventoryModel()
            {
                Inventory = CharacterModel.Inventory,
            });
            await new SaveManager().Save(new SaveCardModel()
            {
                CardCollection = CharacterModel.CardCollection,
                MaxLineupTeamCount = CharacterModel.MaxLineupTeamCount,
                CardLineups = CharacterModel.CardLineups,
            });
            await new SaveManager().Save(new SaveUniqueIdentityModel()
            {
                UniqueIdentity = UniqueIdentityModel,
            });
        }
    }
}