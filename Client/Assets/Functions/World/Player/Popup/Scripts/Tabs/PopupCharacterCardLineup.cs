using System;
using System.Collections.Generic;
using Globals;
using UnityEngine;
using Utils.Tab;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterCardLineup : MonoBehaviour
    {
        [SerializeField] private TabSwitcher _tabSwitcherTeam;
        [SerializeField] private List<LineupCard> _lineupCards;

        private void OnEnable()
        {
            _tabSwitcherTeam.Init();
            InitLineupTeamTab();
            InitCards(1);
        }

        private void Start()
        {
            _tabSwitcherTeam.OnTabSwitched += OnSwitchLineupTeam;
        }

        private void InitLineupTeamTab()
        {
            var maxLineupTeamCount = Global.Instance.Get<CharacterData>().CharacterModel.MaxLineupTeamCount;

            for (int i = 0; i < _tabSwitcherTeam.Tabs.Count; i++)
            {
                var tab = _tabSwitcherTeam.Tabs[i];
                var button = tab.TabSwitcherButton as CardLineupTeamTab;

                bool isLocked = i >= maxLineupTeamCount; // index nhỏ hơn max thì mở khóa
                button.SetLock(isLocked);
            }
        }

        private void InitCards(int teamLineupIndex)
        {
            Debug.Log("Init lineup " + teamLineupIndex);
            for (int i = 0; i < _lineupCards.Count; i++)
            {
                _lineupCards[i].Setup((byte)(i + 1), teamLineupIndex);
            }
        }

        public void OnSwitchLineupTeam(int index)
        {
            var maxLineupTeamCount = Global.Instance.Get<CharacterData>().CharacterModel.MaxLineupTeamCount;
            if (index + 1 > maxLineupTeamCount)
            {
                Debug.Log($"Slot is lock so return to {index - 1}");
                _tabSwitcherTeam.SwitchTab(index - 1);
                return;
            }

            InitCards(index + 1);
        }

        private void OnDestroy()
        {
            _tabSwitcherTeam.OnTabSwitched -= OnSwitchLineupTeam;
        }
    }
}