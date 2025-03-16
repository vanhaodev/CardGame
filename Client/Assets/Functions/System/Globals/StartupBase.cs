using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Globals
{
    public abstract class StartupBase : MonoBehaviour
    {
        protected List<Func<UniTask>> _tasks = new List<Func<UniTask>>();

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
    }
}