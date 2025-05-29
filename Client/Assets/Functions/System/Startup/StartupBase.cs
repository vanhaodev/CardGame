using System;
using System.Collections.Generic;
using System.SceneLoader;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Globals
{
    public abstract class StartupBase : SingletonMonoBehavior<StartupBase>
    {
        protected LoadingTaskProviderModel _taskModel { get; } = new();
        [SerializeField] protected GameObject[] _objEnableAfterDone;

        protected virtual void Awake()
        {
            base.Awake();
        }

        protected override void CustomAwake()
        {
            base.CustomAwake();
        }

        public List<Func<UniTask>> GetTasks() => _taskModel.Tasks;

        public void AddTask(Func<UniTask> task)
        {
            _taskModel.Add(task);
        }

        public void AddTasks(IEnumerable<Func<UniTask>> tasks)
        {
            _taskModel.AddRange(tasks);
        }

        protected virtual async UniTask FinishStartup()
        {
            // Hiển thị UI sau khi hoàn thành
            foreach (var obj in _objEnableAfterDone)
            {
                obj.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}