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

        protected override void CustomAwake()
        {
            Debug.LogError("GlobalBase: Awake");
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

            // Tìm tất cả GlobalNeeder trong scene
            var needers = GameObject.FindObjectsByType<GlobalNeeder>(FindObjectsSortMode.None);
            if (needers != null && needers.Length > 0) // Kiểm tra danh sách không rỗng
            {
                foreach (var needer in needers)
                {
                    var comps = needer.GetNeeders();
                    if (comps == null || comps.Count == 0) continue; // Bỏ qua nếu không có component nào

                    foreach (var comp in comps)
                    {
                        if (comp is IGlobal globalComponent)
                        {
                            var type = comp.GetType();
                            if (!_instances.ContainsKey(type))
                            {
                                _instances[type] = globalComponent; // Thêm vào dictionary nếu chưa có
                            }
                        }
                    }
                }
            }
        }

        public async UniTask WaitForInit<T>() where T : IGlobal
        {
            Debug.LogWarning("GlobalBase: WaitForInit");
            await UniTask.WaitUntil(() => _instances.ContainsKey(typeof(T)));
            Debug.LogWarning("GlobalBase: WaitForInit done");
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

        public virtual async UniTask Init()
        {
            List<UniTask> initTasks = new List<UniTask>();
            foreach (var component in _instances.Values)
            {
                initTasks.Add(component.Init());
            }

            await UniTask.WhenAll(initTasks);
        }
    }
}