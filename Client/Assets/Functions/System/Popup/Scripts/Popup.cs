using Globals;
using UnityEngine;

namespace Popup
{
    public class Popup : MonoBehaviour
    {
        public void Close()
        {
            Debug.Log("Close");
     
            Global.Instance.Get<PopupManager>().ClosePopup(this);
        }
    }
}