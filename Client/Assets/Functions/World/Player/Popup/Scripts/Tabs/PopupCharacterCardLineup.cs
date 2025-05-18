using System;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterCardLineup : MonoBehaviour
    {
        [SerializeField] private TabSwitcher _tabSwitcherTeam;

        private void OnEnable()
        {
            _tabSwitcherTeam.Init();
        }
    }
}