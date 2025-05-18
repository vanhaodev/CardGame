using System;
using System.Collections.Generic;
using System.Threading;
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
        public UnityEvent<int> OnTabSwitched;
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
                if (Tabs[i].TabSwitcherButton != null)
                {
                    tabBtn = Tabs[i].TabSwitcherButton;
                }
                else
                {
                    tabBtn = Instantiate(_prefabTabButton, _parentTabButton);
                }

                Tabs[i].TabSwitcherButton = tabBtn;
                tabBtn.Set( Tabs[i].TabButtonName, Tabs[i].SpriteTabButtonIcon, () => SwitchTab(index));
            }

            SwitchTab(DefaultIndex);
        }

        public void SwitchTab(int index)
        {
            OnTabSwitched?.Invoke(index);
            for (int i = 0; i < Tabs.Count; i++)
            {
                Tabs[i].Set(i == index);
                if (_txWindowTitle && i == index)
                {
                    _ctsCurrentTabTitleTextAnim?.Cancel();
                    _ctsCurrentTabTitleTextAnim = new CancellationTokenSource();
                    _textTweener.AnimateTextChangeAsync(_txWindowTitle, Tabs[i].TabButtonName, 0.01f,
                        _ctsCurrentTabTitleTextAnim);
                }
            }

            CurrentIndex = index;
        }
    }
}