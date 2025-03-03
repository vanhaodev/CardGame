using Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Globals
{
    public class GlobalFunction : SingletonMonoBehavior<GlobalFunction>
    {
        //General

        [SerializeField] AddressableLoader _addressableLoader;
        [SerializeField] SoundManager _soundManager;

        //public
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