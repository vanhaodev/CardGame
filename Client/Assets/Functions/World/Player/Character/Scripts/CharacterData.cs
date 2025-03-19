using System;
using Cysharp.Threading.Tasks;
using Globals;
using Save;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using World.Player.Character;

namespace World.Player.Character
{
    public class CharacterData : MonoBehaviour, IGlobal
    {
        [SerializeField] private CharacterModel _characterModel;
        public readonly Subject<Unit> OnCharacterChanged = new Subject<Unit>();
        public void InvokeOnCharacterChanged() => OnCharacterChanged.OnNext(Unit.Default);
        public CharacterModel CharacterModel
        {
            get => _characterModel;
            set
            {
                _characterModel = value;
                InvokeOnCharacterChanged();
            }
        }

        public async UniTask Init()
        {
            PlayerPrefs.SetInt("IsNewbie", 1);
            Debug.Log("Loading playerData data");
            var save = new SaveManager();
            var playerData = await save.Load<SavePlayerModel>();
            Global.Instance.Get<CharacterData>().CharacterModel = playerData.CharacterModel;
        }
    }   
}