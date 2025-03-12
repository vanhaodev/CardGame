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
        /// <summary>
        /// If compnent want to singleton in other gameobject, just drag ref to _outers
        /// </summary>
        [SerializeField] List<MonoBehaviour> _outers;

        protected override void Awake()
        {
            base.Awake();

            // Duyệt qua tất cả các component trong gameObject hiện tại
            foreach (var component in GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component is IGlobal globalComponent)
                {
                    _instances[component.GetType()] = globalComponent;
                }
            }

            // Duyệt qua các MonoBehaviour trong _outers và thêm vào dictionary
            foreach (var outer in _outers)
            {
                if (outer != null && outer is IGlobal globalComponent) // Kiểm tra xem outer có thực thi IGlobal không
                {
                    _instances[outer.GetType()] = globalComponent; // Thêm vào dictionary
                }
                else
                {
                    // Log lỗi nếu không phải IGlobal
                    Debug.LogError(
                        $"❌ Component {outer.GetType()} không thực thi IGlobal và sẽ không được thêm vào dictionary.");
                }
            }
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