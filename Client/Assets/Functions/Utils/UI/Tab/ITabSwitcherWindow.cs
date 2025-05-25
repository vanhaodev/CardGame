using Cysharp.Threading.Tasks;

namespace Utils.Tab
{
    public interface ITabSwitcherWindow
    {
        public UniTask Init(TabSwitcherWindowModel model = null);
    }
}