using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Globals;
using Newtonsoft.Json;
using UnityEngine;

namespace System.Analytics
{
    public class Analytics : MonoBehaviour, IGlobal
    {
        private FirebaseApp app;

        void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    app = FirebaseApp.DefaultInstance;
                    Debug.Log("🔥 Firebase Initialized Successfully!");

                    // Sau khi Firebase sẵn sàng, gửi event test
                    LogTestEvent();
                }
                else
                {
                    Debug.LogError($"❌ Firebase Initialization Failed: {dependencyStatus}");
                }
            });
        }

        void LogTestEvent()
        {
            // Debug.Log("🚀 Sending Firebase Event...");
            // FirebaseAnalytics.LogEvent("test", "message", "HÀO ĐẤNG IS COMING");
            // Debug.Log("✅ Event Sent!");
            // TracingIAP("gold", "9.99");
        }

        public void Send(string key, string value)
        {
            FirebaseAnalytics.LogEvent("log", key, value);
        }

        public void TracingIAP(string key, string value = "0.99")
        {
            FirebaseAnalytics.LogEvent("IAP", key, value);
            Debug.Log($"TracingIAP <color=red>{JsonConvert.SerializeObject(new string[] { key, value })}");
        }

        public void FindGame(string key)
        {
            FirebaseAnalytics.LogEvent(key.ToLower());
            Debug.Log($"FindGame <color=red>{JsonConvert.SerializeObject(new string[] { key })}");
        }

        public void UseSkill(string skillName, string cell)
        {
            FirebaseAnalytics.LogEvent("use_skill", skillName, cell);
            Debug.Log(
                $"UseSkill <color=red>{JsonConvert.SerializeObject(new string[] { "use_skill", skillName, cell })}");
        }

        /// <summary>
        /// the player exit room while playing => penalty
        /// </summary>
        public void QuitGame()
        {
            FirebaseAnalytics.LogEvent("quit_game");
        }

        public async UniTask Init()
        {
        }
    }
}