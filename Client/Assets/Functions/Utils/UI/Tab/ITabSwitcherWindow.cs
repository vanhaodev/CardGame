using Cysharp.Threading.Tasks;

namespace Utils.Tab
{
    public interface ITabSwitcherWindow
    {
        public UniTask Init(TabSwitcherWindowModel model = null);
        /// <summary>
        /// After enable
        /// </summary>
        /// <returns></returns>
        public UniTask LateInit();
    }
}