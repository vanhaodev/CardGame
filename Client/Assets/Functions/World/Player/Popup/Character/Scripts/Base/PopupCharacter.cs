using Cysharp.Threading.Tasks;
using Popups;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacter : Popup
    {
        [SerializeField] private TabSwitcher _tabSwitcher;
        private int _switchIndex;
        public override async UniTask SetupData()
        {
            await base.SetupData();
        }

        public void SetSwitchIndexFirst(int switchIndex)
        {
            _switchIndex = switchIndex;
        }
        public override async UniTask Show(float fadeDuration = 0.3f)
        {
            await base.Show(fadeDuration);
            _tabSwitcher.Init(switchIndex: _switchIndex);
        }
        
    }
}