using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace System.SceneLoader
{
    public class LoadingTaskProviderModel
    {
        public List<Func<UniTask>> Tasks { get; } = new();

        public LoadingTaskProviderModel(params Func<UniTask>[] tasks)
        {
            if (tasks != null)
            {
                Tasks.AddRange(tasks);
            }
        }

        public LoadingTaskProviderModel Add(Func<UniTask> task)
        {
            if (task != null)
            {
                Tasks.Add(task);
            }
            return this;
        }

        public LoadingTaskProviderModel AddRange(IEnumerable<Func<UniTask>> tasks)
        {
            if (tasks != null)
            {
                Tasks.AddRange(tasks);
            }
            return this;
        }
    }
}