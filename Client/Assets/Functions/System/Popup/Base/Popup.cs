using System;
using Globals;
using UnityEngine;

namespace Popup
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Popup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        protected virtual void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Close()
        {
            Debug.Log("Close");
     
            Global.Instance.Get<PopupManager>().ClosePopup(this);
        }
    }
}