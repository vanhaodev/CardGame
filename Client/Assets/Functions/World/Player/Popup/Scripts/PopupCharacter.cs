using Cysharp.Threading.Tasks;
using Popups;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacter : Popup
    {
        [SerializeField] private TabSwitcher _tabSwitcher;
        public override async UniTask SetupData()
        {
            await base.SetupData();
            _tabSwitcher.Init();
        }
    }
}