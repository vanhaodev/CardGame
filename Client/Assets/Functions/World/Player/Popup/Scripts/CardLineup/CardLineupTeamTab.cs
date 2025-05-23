using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class CardLineupTeamTab : TabSwitcherButton
    {
        [SerializeField] GameObject _objLock;

        public void SetLock(bool isLock)
        {
            _txBtnName.gameObject.SetActive(!isLock);
            _objLock.SetActive(isLock);
            
        }
    }
}