using DG.Tweening;
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
        [HideInInspector] public TabSwitcherButton TabSwitcherButton;

        public void Set(bool isSelected)
        {
            if (isSelected)
            {
                // Đặt scale ban đầu là 0.2 rồi mới bật đối tượng
                TabSwitcherButton.ObjSelectingCover.transform.localScale =
                    Vector3.one * 0.2f; // Đặt scale ban đầu là 0.2
                TabSwitcherButton.Select(true);
                ObjWindow.SetActive(true); // Bật đối tượng

                // Thực hiện tween scale từ 0.2 lên 1 trong 0.3s
                TabSwitcherButton.ObjSelectingCover.transform.DOScale(Vector3.one, 0.3f);
            }
            else
            {
                ObjWindow.SetActive(false); // Tắt đối tượng sau khi tween hoàn tất
                // Scale ngược lại từ 1 về 0.2 rồi tắt đối tượng
                TabSwitcherButton.ObjSelectingCover.transform.DOScale(Vector3.one * 0.2f, 0.3f).OnComplete(() =>
                {
                    TabSwitcherButton.ObjSelectingCover.transform.localScale =
                        Vector3.one; // Đảm bảo set lại scale về 1
                    TabSwitcherButton.Select(false);
                });
            }
        }
    }
}