using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        public void Init()
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
                if (Tabs[i].TabSwitcherButton == null || Tabs[i].TabSwitcherButton.Equals(null))
                {
                    tabBtn = Instantiate(_prefabTabButton, _parentTabButton);
                }
                else
                {
                    tabBtn = Tabs[i].TabSwitcherButton;
                }

                Tabs[i].TabSwitcherButton = tabBtn;
                tabBtn.Set(Tabs[i].TabButtonName, Tabs[i].SpriteTabButtonIcon, () => SwitchTab(index));
            }

            SwitchTab(DefaultIndex);
        }

        public void SwitchTab(int index)
        {

            for (int i = 0; i < Tabs.Count; i++)
            {
                Tabs[i].Set(i == index);
                if (_txWindowTitle && i == index)
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
            Debug.Log($"SwitchLineupTeam {index}");
            CurrentIndex = index;
            OnTabSwitched?.Invoke(index);
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