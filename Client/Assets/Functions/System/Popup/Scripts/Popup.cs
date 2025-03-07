using Globals;
using UnityEngine;

namespace Popup
{
    public class Popup : MonoBehaviour
    {
        public void Close()
        {
            Debug.Log("Close");
     
            GlobalFunction.Instance.PopupManager.ClosePopup(this);
        }
    }
}