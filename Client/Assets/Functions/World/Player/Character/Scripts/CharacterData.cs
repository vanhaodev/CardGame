using System;
using Cysharp.Threading.Tasks;
using Functions.World.Data;
using Globals;
using Save;
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
            var playerData = await save.Load<SavePlayerModel>();
            CharacterModel = playerData.CharacterModel;
            UniqueIdentityModel = playerData.UniqueIdentityModel;
        }
    }
}