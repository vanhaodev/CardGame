using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utils;

namespace System.SceneLoader
{
    public class SceneLoader : MonoBehaviour, IGlobal
    {
        [SerializeField] private SceneLoaderUI _ui;

        [Button]
        public void Test()
        {
            // LoadScene(1, () => 
            //     new List<Func<UniTask>>()
            //     {
            //      ()=>   Global.Instance.WaitForInit<GameStartup>()
            //     }.Concat(Global.Instance.Get<GameStartup>().GetTasks()).ToList()
            // );
        }

        public async UniTask LoadScene(int buildIndex, LoadingTaskProviderModel afterLoadedSceneloadingTaskProvider,
            UnityAction<float> progressCallback = null)
        {
            await _ui.Show(true);
            // Debug.Log("Loading Scene " + buildIndex);
            // 1️⃣ Load scene async, chiếm 30% tiến trình
            var loadOperation = SceneManager.LoadSceneAsync(buildIndex);
            loadOperation.allowSceneActivation = false;

            // Chờ đến khi tiến trình đạt 90% (0.9)
            while (loadOperation.progress < 0.9f)
            {
                float sceneProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                float totalProgress = sceneProgress * 0.3f;
                progressCallback?.Invoke(totalProgress);
                _ui.SetProgress(sceneProgress);
                await UniTask.Yield();
            }

            // 3️⃣ Kích hoạt scene khi tất cả đã hoàn thành
            loadOperation.allowSceneActivation = true;
            // **Đợi scene kích hoạt hoàn toàn trước khi gọi GameStartup**
            await UniTask.WaitUntil(() => SceneManager.GetActiveScene().buildIndex == buildIndex);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            //=============================STARTUP======================================//
            var startup = FindAnyObjectByType<StartupBase>();
            if (startup != null)
            {
                var tasks = startup.GetTasks();
                int totalTasks = tasks.Count;
                // Debug.Log($"Total tasks: {totalTasks}");

                int i = 0;
                float baseProgress = 0.3f; // Phần trăm ban đầu của progress
                float maxTaskProgress = 0.35f; // Phần trăm tối đa dành cho task
                foreach (var loadingTask in tasks)
                {
                    i++;
                    // Debug.Log($"Executing task {i}/{totalTasks}");

                    if (loadingTask != null)
                    {
                        try
                        {
                            await loadingTask();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error executing task: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Task is null!");
                    }  
                    // Giảm số lần cập nhật progress để tránh overload UI
                    float taskProgress = (i / (float)totalTasks) * maxTaskProgress;
                    float currentProgress = baseProgress + taskProgress;

                    // if (i == totalTasks || i % 5 == 0) // Cập nhật mỗi 5 task hoặc task cuối
                    // {
                    //     progressCallback?.Invoke(currentProgress);
                    //     _ui.SetProgress(currentProgress);
                    // }
                    progressCallback?.Invoke(currentProgress);
                    _ui.SetProgress(currentProgress);
                }
            }

            //==================afterLoadedSceneloadingTaskProvider===============//
            // Debug.Log($"Loading Task scene: {buildIndex}");

            // Kiểm tra null trước khi truy cập Tasks
            if (afterLoadedSceneloadingTaskProvider == null || afterLoadedSceneloadingTaskProvider.Tasks == null || !afterLoadedSceneloadingTaskProvider.Tasks.Any())
            {
                // Debug.LogWarning("No tasks to load.");
            }
            else
            {
                var tasks = afterLoadedSceneloadingTaskProvider.Tasks;
                int totalTasks = tasks.Count;
                Debug.Log($"Total tasks: {totalTasks}");

                int i = 0;
                float baseProgress = 0.65f; // Phần trăm ban đầu của progress
                float maxTaskProgress = 0.35f; // Phần trăm tối đa dành cho task

                foreach (var loadingTask in tasks)
                {
                    i++;
                    Debug.Log($"Executing task {i}/{totalTasks}");

                    if (loadingTask != null)
                    {
                        try
                        {
                            await loadingTask();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error executing task {i}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Task {i} is null!");
                    }

                    // Giảm số lần cập nhật progress để tránh overload UI
                    float taskProgress = (i / (float)totalTasks) * maxTaskProgress;
                    float currentProgress = baseProgress + taskProgress;

                    // if (i == totalTasks || i % 5 == 0) // Cập nhật mỗi 5 task hoặc task cuối
                    // {
                    //     progressCallback?.Invoke(currentProgress);
                    //     _ui.SetProgress(currentProgress);
                    // }
                    progressCallback?.Invoke(currentProgress);
                    _ui.SetProgress(currentProgress);
                }
            }

            //=-----------------------------------------------------=//


            // Debug.Log("Preparing scene: " + buildIndex);
            progressCallback?.Invoke(1);
            _ui.SetProgress(1);
            await _ui.WaitFillAmountFull();
            await _ui.Show(false);
        }

        public async UniTask Init()
        {
            // await _ui.Show(false);
            // gameObject.SetActive(false);
        }
    }
}