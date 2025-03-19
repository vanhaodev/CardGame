using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Globals
{
    public abstract class GlobalBase<T> : SingletonMonoBehavior<T> where T : MonoBehaviour
    {
        /// <summary>
        /// Just create gameobject child and add compnent => Global will create instance
        /// </summary>
        private Dictionary<Type, IGlobal> _instances = new();

        [SerializeField] List<string> _importedLogs = new();
        [SerializeField] List<string> _initialed = new List<string>();
        private bool _isLoadedAllComp = false;

        protected override void CustomAwake()
        {
            _isLoadedAllComp = false;
            Debug.Log("GlobalBase: Awake");
            base.CustomAwake();
            _instances = _instances.Where(pair => pair.Value != null)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            // Duyệt qua tất cả các component trong gameObject hiện tại
            foreach (var component in GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component is IGlobal globalComponent)
                {
                    _instances[component.GetType()] = globalComponent;
                }
            }

            UpdateImportedLogs();
            _isLoadedAllComp = true;
        }

        public void AddNeeder(GlobalNeeder needer)
        {
            var comps = needer.GetNeeders();
            if (comps == null || comps.Count == 0) return;

            foreach (var comp in comps)
            {
                if (comp is IGlobal globalComponent)
                {
                    var type = comp.GetType();
                    _instances[type] = globalComponent; // Thêm vào dictionary nếu chưa có
                }
            }

            UpdateImportedLogs();
        }

        public async UniTask WaitForInit<T>() where T : IGlobal
        {
            Debug.LogWarning("GlobalBase: WaitForInit " + typeof(T).Name);
            await UniTask.WaitUntil(() => _initialed.Contains(typeof(T).ToString()));
            Debug.LogWarning("GlobalBase: WaitForInit done " + typeof(T).Name);
        }

        public TComponent Get<TComponent>() where TComponent : class, IGlobal
        {
            if (_instances.TryGetValue(typeof(TComponent), out var instance))
            {
                return instance as TComponent;
            }

            Debug.LogError($"❌ {typeof(T).Name}: Instance of {typeof(TComponent).Name} not found!");
            return null;
        }

        private void UpdateImportedLogs()
        {
            _importedLogs = _instances.Select(i => i.Key.ToString()).ToList();
        }

        public virtual async UniTask Init()
        {
            await UniTask.WaitUntil(() => _isLoadedAllComp);

            List<UniTask> initTasks = new List<UniTask>();
            foreach (var component in _instances.Values)
            {
                if (!_initialed.Contains(component.GetType().ToString()))
                {
                    initTasks.Add(component.Init());
                }
            }

            await UniTask.WhenAll(initTasks);
            _initialed = _instances.Select(i => i.Value.GetType().ToString()).ToList();
            _isLoadedAllComp = false;
            Debug.Log("GlobalBase: Init");
        }
    }
}