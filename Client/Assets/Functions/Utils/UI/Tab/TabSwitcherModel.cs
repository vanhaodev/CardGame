using UnityEngine;
using UnityEngine.UI;

namespace Utils.Tab
{
    [System.Serializable]
    public class TabSwitcherModel
    {
        public GameObject ObjWindow;
        public string TabButtonName;
        public Sprite SpriteTabButtonIcon;
        public TabSwitcherButton TabSwitcherButton;
    }
}