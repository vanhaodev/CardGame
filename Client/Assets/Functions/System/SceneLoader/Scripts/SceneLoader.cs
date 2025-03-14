using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utils;

namespace System.SceneLoader
{
    public class SceneLoader : SingletonMonoBehavior<SceneLoader>
    {
        [SerializeField] private SceneLoaderUI _ui;

        [Button]
        public void Test()
        {
            LoadScene(1, () => new List<Func<UniTask>>() // Chỉ tạo danh sách task sau khi scene load xong
            {
                () => GameStartup.Instance.WaitGlobal(),
                () => UniTask.Delay(1000),
                () => GameStartup.Instance.InitGlobal(),
                () => UniTask.Delay(1000),
                () => GameStartup.Instance.LoadSaveData(),
                () => UniTask.Delay(1000),
                () => GameStartup.Instance.FinishStartup(),
                () => UniTask.Delay(1000),
            });
        }

        public async UniTask LoadScene(int buildIndex, Func<List<Func<UniTask>>> loadingTaskProvider,
            UnityAction<float> progressCallback = null)
        {
            await _ui.Show(true);

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

            // 2️⃣ Lấy danh sách task sau khi scene đã load xong
            var loadingTasks = loadingTaskProvider();
            int totalTasks = loadingTasks.Count;
            for (int i = 0; i < totalTasks; i++)
            {
                await loadingTasks[i](); // Gọi hàm tạo UniTask tại thời điểm này
                float taskProgress = ((i + 1) / (float)totalTasks) * 0.7f;
                progressCallback?.Invoke(0.3f + taskProgress);
                _ui.SetProgress(0.3f + taskProgress);
            }

            progressCallback?.Invoke(1);
            _ui.SetProgress(1);

            await _ui.Show(false);
        }
    }
}