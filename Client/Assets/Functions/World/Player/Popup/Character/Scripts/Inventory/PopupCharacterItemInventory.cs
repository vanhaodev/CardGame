using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterItemInventory : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] private InventoryUI _inventoryUI;
        public UniTask Init(TabSwitcherWindowModel model = null)
        {
            return _inventoryUI.Init(new (), new());
        }

        public UniTask LateInit()
        {
            return UniTask.CompletedTask;
        }
    }
}