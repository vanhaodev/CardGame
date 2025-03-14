using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace System.SceneLoader
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneLoaderUI _ui;

        public async UniTask LoadScene(int buildIndex, List<UniTask> loadingTasks,
            UnityAction<float> progressCallback = null)
        {
            await _ui.Show(true);

            // 1️⃣ Load scene async, chiếm 30% tiến trình
            var loadOperation = SceneManager.LoadSceneAsync(buildIndex);
            loadOperation.allowSceneActivation = false; // Không kích hoạt ngay
            while (!loadOperation.isDone)
            {
                float sceneProgress = Mathf.Clamp01(loadOperation.progress / 0.9f); // Tiến trình load scene (0 → 1)
                float totalProgress = sceneProgress * 0.3f; // Load scene chiếm 30%
                progressCallback?.Invoke(totalProgress);
                _ui.SetProgress(sceneProgress);
                await UniTask.Yield();
            }

            // 2️⃣ Load các task còn lại, chiếm 70% tiến trình
            int totalTasks = loadingTasks.Count;
            for (int i = 0; i < totalTasks; i++)
            {
                await loadingTasks[i];
                float taskProgress = ((i + 1) / (float)totalTasks) * 0.7f; // Task chiếm 70%
                progressCallback?.Invoke(0.3f + taskProgress);
                _ui.SetProgress(0.3f + taskProgress);
            }

            progressCallback?.Invoke(1);
            _ui.SetProgress(1);
            // 3️⃣ Kích hoạt scene khi tất cả đã hoàn thành
            loadOperation.allowSceneActivation = true;

            await _ui.Show(false);
        }
    }
}