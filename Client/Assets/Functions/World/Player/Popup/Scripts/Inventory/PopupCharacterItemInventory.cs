using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterItemInventory : MonoBehaviour, ITabSwitcherWindow
    {
        public UniTask Init(TabSwitcherWindowModel model = null)
        {
            return UniTask.CompletedTask;
        }

        public UniTask LateInit()
        {
            return UniTask.CompletedTask;
        }
    }
}