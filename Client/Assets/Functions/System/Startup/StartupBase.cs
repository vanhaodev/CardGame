using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Globals
{
    public abstract class StartupBase : SingletonMonoBehavior<StartupBase>
    {
        protected List<Func<UniTask>> _tasks = new List<Func<UniTask>>();
        [SerializeField] protected GameObject[] _objEnableAfterDone;
        protected virtual void Awake()
        {
        }

        public List<Func<UniTask>> GetTasks() => _tasks;

        public void AddTask(Func<UniTask> task)
        {
            _tasks.Add(task);
        }

        public void AddTasks(List<Func<UniTask>> tasks)
        {
            _tasks.AddRange(tasks);
        }
        protected virtual async UniTask FinishStartup()
        {
            //show the ui
            foreach (var obj in _objEnableAfterDone)
            {
                obj.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}