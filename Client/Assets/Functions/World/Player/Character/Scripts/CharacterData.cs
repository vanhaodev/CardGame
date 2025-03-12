using System;
using Cysharp.Threading.Tasks;
using Globals;
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

        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }   
}