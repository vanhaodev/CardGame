using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Globals
{
    public class HomeStartup : StartupBase
    {
        protected override async void Awake()
        {
            base.Awake();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
#endif
#if UNITY_EDITOR
            Application.targetFrameRate = 120;
#endif
            await UniTask.WaitUntil(() => Global.Instance != null);
            await Global.Instance.Init();
            await Global.Instance.WaitForInit<SoundManager>();
            Global.Instance.Get<SoundManager>().PlaySoundLoop("5", 1);
            await FinishStartup();
        }
    }
}