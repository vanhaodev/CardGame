using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils.Tab
{
    public class TabSwitcherButton : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _txBtnName;
        [SerializeField] protected Image _imgIcon;
        [SerializeField] protected Button _btn;
        [SerializeField] protected GameObject _objSelectingCover;
        public GameObject ObjSelectingCover => _objSelectingCover;

        public void SetButtonActive(bool active)
        {
            _btn.interactable = active;
        }

        public void Set(string btnName, Sprite icon, UnityAction onClick)
        {
            if (_txBtnName != null)
            {
                _txBtnName.text = btnName;
            }

            if (icon != null)
            {
                if (_imgIcon != null)
                {
                    _imgIcon.enabled = true;
                    _imgIcon.sprite = icon;
                }
            }
            else
            {
                if (_imgIcon != null)
                {
                    _imgIcon.sprite = null;
                    _imgIcon.enabled = false;
                }
            }

            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(onClick);
            Select(false);
        }

        public void Select(bool isSelected)
        {
            _objSelectingCover.SetActive(isSelected);
            _btn.enabled = !isSelected;
        }
    }
}