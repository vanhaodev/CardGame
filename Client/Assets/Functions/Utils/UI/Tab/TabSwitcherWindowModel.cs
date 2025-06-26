using System;
using UnityEngine.Events;

namespace Utils.Tab
{
    public class TabSwitcherWindowModel
    {
        /// <summary>
        /// Change khi hoàn thành công việc
        /// Dành cho các tab có những hành động có thể thay đổi data, thì khi onchange sẽ giúp tab chính cập nhật lại hiển thị đúng
        /// </summary>
        public UnityAction OnChanged;
        /// <summary>
        /// Change liên tục ở công việc đang làm
        /// </summary>
        public UnityAction OnRegularChanged;
    }
}