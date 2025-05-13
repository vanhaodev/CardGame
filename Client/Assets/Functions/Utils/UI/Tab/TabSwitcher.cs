using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Tab
{
    public class TabSwitcher : MonoBehaviour
    {
        public List<TabSwitcherModel> Tabs = new List<TabSwitcherModel>();
        [SerializeField] TabSwitcherButton _prefabTabButton;
        [SerializeField] Transform _parentTabButton;
        public int DefaultIndex;
        public int CurrentIndex;

        public void Start()
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
                Tabs[i].ObjWindow.SetActive(i == index);
                Tabs[i].TabSwitcherButton.Select(i == index);
            }

            CurrentIndex = index;
        }
    }
}