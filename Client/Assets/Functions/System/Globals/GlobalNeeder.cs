using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Globals
{
    /// <summary>
    /// Nếu một Component A được GetComponent hoặc Reference vào một Component B nằm trên GameObject có DontDestroyOnLoad,<br/>
    /// nhưng bản thân A KHÔNG nằm trong cùng GameObject với B, thì A SẼ KHÔNG được giữ lại khi chuyển scene.
    /// </summary>
    public class GlobalNeeder : MonoBehaviour
    {
        /// <summary>
        /// If compnent want to singleton in other gameobject, just drag ref to _outers
        /// </summary>
        [SerializeField] List<MonoBehaviour> _needers;
        public List<MonoBehaviour> GetNeeders() => _needers;

        private async void Start()
        {
            await UniTask.WaitUntil(()=>Global.Instance != null);
            Global.Instance.AddNeeder(this);
        }
        private void OnDestroy()
        {
            if (Global.Instance == null) return;

            foreach (var x in _needers)
            {
                if (x is IGlobal global)
                {
                    Global.Instance.RemoveComponent(global);
                    // Debug.Log("Remove " + x.GetType().Name + " from Global");
                }
            }
        }
    }

}