using Utils;
using Cysharp.Threading.Tasks;
using FloatingEffect;
using Popup;
using UnityEngine;
using World.Board;

namespace Globals
{
    public class GlobalFunction : SingletonMonoBehavior<GlobalFunction>
    {
        //General

        [SerializeField] AddressableLoader _addressableLoader;
        [SerializeField] SoundManager _soundManager;
        [SerializeField] PopupManager _popupManager;

        public PopupManager PopupManager
        {
            get => _popupManager;
            set => _popupManager = value;
        }
//public

        //world
        [SerializeField] BoardCommander _boardCommander;
        [SerializeField] FloatingEffectManager _floatingEffectManager;

        public BoardCommander BoardCommander
        {
            get => _boardCommander;
            set => _boardCommander = value;
        }

        public FloatingEffectManager FloatingEffectManager
        {
            get => _floatingEffectManager;
            set => _floatingEffectManager = value;
        }

        public AddressableLoader AddressableLoader
        {
            get => _addressableLoader;
            set => _addressableLoader = value;
        }

        public SoundManager SoundManager
        {
            get => _soundManager;
            set => _soundManager = value;
        }

        public async UniTask Init()
        {
            await _soundManager.Init();
        }
    }
}