using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils.Tab
{
    [System.Serializable]
    public class TabSwitcherModel : IDisposable
    {
        public GameObject ObjWindow;
        public string TabButtonName;
        public Sprite SpriteTabButtonIcon;
        /// <summary>
        /// Nếu null thì hệ thống sẽ sinh ra từ prefab ở TabSwicher <br/>
        /// muốn tab đứng ở các vị trí đặc biệt mà hệ thống scrollview hay grid không làm được thì có thể set sẵn rồi kéo vào để ko pải instantiate
        /// </summary>
        public TabSwitcherButton TabSwitcherButton;
        CancellationTokenSource _ctsSelectingBorderAnim;
        public void Set(bool isSelected)
        {
            _ctsSelectingBorderAnim?.Cancel();
            _ctsSelectingBorderAnim = new CancellationTokenSource();
            if (isSelected)
            {
                // Đặt scale ban đầu là 0.2 rồi mới bật đối tượng
                TabSwitcherButton.ObjSelectingCover.transform.localScale =
                    Vector3.zero; // Đặt scale ban đầu là 0.2
                TabSwitcherButton.Select(true);
                ShowWindow(true); // Bật đối tượng

                // Thực hiện tween scale từ 0.2 lên 1 trong 0.3s
                TabSwitcherButton.ObjSelectingCover.transform.DOScale(Vector3.one, 0.3f)
                    .WithCancellation(cancellationToken: _ctsSelectingBorderAnim.Token).Forget();
            }
            else
            {
                ShowWindow(false); // Tắt đối tượng sau khi tween hoàn tất
                // Scale ngược lại từ 1 về 0.2 rồi tắt đối tượng
                TabSwitcherButton.ObjSelectingCover.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                {
                    TabSwitcherButton.ObjSelectingCover.transform.localScale =
                        Vector3.one; // Đảm bảo set lại scale về 1
                    TabSwitcherButton.Select(false);
                }).WithCancellation(cancellationToken: _ctsSelectingBorderAnim.Token).Forget();
            }
        }

        public void ShowWindow(bool isShow = true)
        {
            if (ObjWindow)
            {
                ObjWindow.SetActive(isShow); // Bật đối tượng
            }
        }

        public void Dispose()
        {
            _ctsSelectingBorderAnim?.Cancel();
        }
    }
}