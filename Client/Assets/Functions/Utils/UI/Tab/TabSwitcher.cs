using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI.DOTween;

namespace Utils.Tab
{
    public class TabSwitcher : MonoBehaviour
    {
        public List<TabSwitcherModel> Tabs = new List<TabSwitcherModel>();
        [SerializeField] TabSwitcherButton _prefabTabButton;
        [SerializeField] Transform _parentTabButton;
        [SerializeField] private TextMeshProUGUI _txWindowTitle;
        public int DefaultIndex;
        public int CurrentIndex;
        DOTweenTextFadeSmooth _textTweener = new DOTweenTextFadeSmooth();

        public void Init()
        {
            if (Tabs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _parentTabButton.childCount; i++)
            {
                Transform child = _parentTabButton.GetChild(i);
                Destroy(child.gameObject);
            }

            for (int i = 0; i < Tabs.Count; i++)
            {
                int index = i;
                var tabBtn = Instantiate(_prefabTabButton, _parentTabButton);
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
                    _textTweener.AnimateTextChangeAsync(_txWindowTitle, Tabs[i].TabButtonName, 0.01f);
                }
            }

            CurrentIndex = index;
        }
    }
}