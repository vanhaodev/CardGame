using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Player.PopupCharacter
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] Image _imgBackground;
        [SerializeField] private Image _imgItemIcon;
        [SerializeField] private TextMeshProUGUI _txItemQuantity;
        [SerializeField] private GameObject _objLoadingLock;
        [SerializeField] Sprite[] _spriteBackgrounds;
        [SerializeField] private uint _itemId;
        [SerializeField] private byte _itemType;
        public uint ItemId => _itemId;
        public async UniTask Init()
        {
            _objLoadingLock.SetActive(true);
            _objLoadingLock.SetActive(false);
        }
        public void OnTouch()
        {
            if(_objLoadingLock.activeSelf) return;
        }

        public void OnHold()
        {
            if(_objLoadingLock.activeSelf) return;
        }
    }

}