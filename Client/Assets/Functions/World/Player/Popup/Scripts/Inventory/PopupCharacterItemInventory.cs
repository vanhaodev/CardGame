using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterItemInventory : MonoBehaviour, ITabSwitcherWindow
    {
        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }
}