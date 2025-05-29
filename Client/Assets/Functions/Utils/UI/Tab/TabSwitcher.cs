using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.UI.DOTween;

namespace Utils.Tab
{
    public class TabSwitcher : MonoBehaviour
    {
        public List<TabSwitcherModel> Tabs = new List<TabSwitcherModel>();
        public Action<int> OnTabSwitched;
        public UnityEvent<int> OnTabSwitchedEvent;
        [SerializeField] TabSwitcherButton _prefabTabButton;
        [SerializeField] Transform _parentTabButton;
        [SerializeField] private TextMeshProUGUI _txWindowTitle;
        /// <summary>
        /// sinh tab button or dùng tab có sẵn
        /// </summary>
        [SerializeField] private bool _isInstantiateTabButton;

        public int DefaultIndex;
        public int CurrentIndex;
        DOTweenTextFadeSmooth _textTweener = new DOTweenTextFadeSmooth();
        CancellationTokenSource _ctsCurrentTabTitleTextAnim;

        public void Init(TabSwitcherWindowModel model = null, int switchIndex = -1)
        {
            if (Tabs.Count == 0)
            {
                return;
            }

            if (_isInstantiateTabButton)
            {
                foreach (var tab in Tabs)
                {
                    tab.TabSwitcherButton = null;
                }

                for (int i = 0; i < _parentTabButton.childCount; i++)
                {
                    Transform child = _parentTabButton.GetChild(i);

                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < Tabs.Count; i++)
            {
                int index = i;
                TabSwitcherButton tabBtn = null;
                if (Tabs[i].TabSwitcherButton == null)
                {
                    tabBtn = Instantiate(_prefabTabButton, _parentTabButton);
                }
                else
                {
                    tabBtn = Tabs[i].TabSwitcherButton;
                }

                Tabs[i].TabSwitcherButton = tabBtn;
                var theModel = model;
                tabBtn.Set(_isInstantiateTabButton, Tabs[i].TabButtonName, Tabs[i].SpriteTabButtonIcon, () => SwitchTab(index, theModel));
            }

            SwitchTab(switchIndex >= 0 ? switchIndex : DefaultIndex, model);
        }

        public async void SwitchTab(int index, TabSwitcherWindowModel model = null)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                bool isSelect = i == index;
                if (isSelect && Tabs[i].ObjWindow != null)
                {
                    //chỉ chạy khi hệ thống tab có sử dụng window, một vài loại dùng event để check tab thay vì hiện window
                    var iTab = Tabs[i].ObjWindow?.GetComponent<ITabSwitcherWindow>();
                    if(iTab != null) await iTab.Init(model);
                    Tabs[i].Set(true);
                    if (iTab != null) await iTab.LateInit();
                }
                else
                {
                    Tabs[i].Set(isSelect);
                }

                if (_txWindowTitle && isSelect)
                {
                    _ctsCurrentTabTitleTextAnim?.Cancel();
                    _ctsCurrentTabTitleTextAnim = new CancellationTokenSource();
                    if (_txWindowTitle)
                    {
                        _textTweener.AnimateTextChangeAsync(_txWindowTitle, Tabs[i].TabButtonName, 0.01f,
                            _ctsCurrentTabTitleTextAnim).Forget();
                    }
                }
            }

            // Debug.Log($"SwitchTab {index}");
            CurrentIndex = index;
            OnTabSwitched?.Invoke(index);
            OnTabSwitchedEvent?.Invoke(index);
        }

        private void OnDestroy()
        {
            foreach (var tab in Tabs)
            {
                tab.Dispose();
            }

            _ctsCurrentTabTitleTextAnim?.Cancel();
        }
    }
}