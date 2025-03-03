using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Utils
{
    public class SingletonNonMonoBehavior<T> where T : new()
    {
        //private static T _instance;

        //public static T Instance => _instance ??= new T();
        private static readonly T _instance = new T();
        public static T Instance => _instance;
    }
    public class SingletonMonoBehavior<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance => instance;

        [SerializeField] protected bool dontDestroyOverload;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;

                if (dontDestroyOverload)
                {
                    DontDestroyOnLoad(gameObject);
                }

                CustomAwake();
            }
            else
            {
                Debug.LogWarning($"Singleton conflict in scene: {gameObject.scene.name} and Instance == {(Instance == null ? "null" : "have")}", gameObject);
                Destroy(gameObject);
                
            }
        }

        protected virtual void CustomAwake() { }
    }
}