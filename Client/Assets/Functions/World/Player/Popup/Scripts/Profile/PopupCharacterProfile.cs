using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterProfile : MonoBehaviour, ITabSwitcherWindow
    {
        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }
}