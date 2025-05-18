using System;
using Globals;
using UnityEngine;
using Utils.Tab;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterCardLineup : MonoBehaviour
    {
        [SerializeField] private TabSwitcher _tabSwitcherTeam;

        private void OnEnable()
        {
            _tabSwitcherTeam.Init();
            InitLineupTeamTab();
        }

        private void Start()
        {
            _tabSwitcherTeam.OnTabSwitched += OnSwitchLineupTeam;
        }

        private void InitLineupTeamTab()
        {
            var maxLineupTeamCount = 2; //Global.Instance.Get<CharacterData>().CharacterModel.MaxLineupTeamCount;

            for (int i = 0; i < _tabSwitcherTeam.Tabs.Count; i++)
            {
                var tab = _tabSwitcherTeam.Tabs[i];
                var button = tab.TabSwitcherButton as CardLineupTeamTab;

                bool isLocked = i >= maxLineupTeamCount; // index nhỏ hơn max thì mở khóa
                button.SetLock(isLocked);
            }
        }

        public void OnSwitchLineupTeam(int index)
        {
            var maxLineupTeamCount = 2;
            if (index + 1 > maxLineupTeamCount)
            {
                Debug.Log($"Slot is lock so return to {index - 1}");
                _tabSwitcherTeam.SwitchTab(index - 1);
            }
        }

        private void OnDestroy()
        {
            _tabSwitcherTeam.OnTabSwitched -= OnSwitchLineupTeam;
        }
    }
}