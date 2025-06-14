using System;
using Globals;
using Popups;
using UnityEngine;

namespace World.Lobby
{
    public class Lobby : MonoBehaviour
    {
        public void OpenSetting()
        {
            Global.Instance.Get<PopupManager>().ShowSetting();
        }
        public void OpenLineup()
        {
            Global.Instance.Get<PopupManager>().ShowCharacter(0);
        }

        public void OpenCollection()
        {
            Global.Instance.Get<PopupManager>().ShowCharacter(1);
        }

        public void OpenInventory()
        {
            Global.Instance.Get<PopupManager>().ShowCharacter(2);
        }

        public void OpenProfile()
        {
            Global.Instance.Get<PopupManager>().ShowCharacter(3);
        }
        public void OpenGachaCard()
        {
            Global.Instance.Get<PopupManager>().ShowGachaCard();
        }
        public void OpenGachaEquipment()
        {
            Global.Instance.Get<PopupManager>().ShowGachaEquipment();
        }
    }
}