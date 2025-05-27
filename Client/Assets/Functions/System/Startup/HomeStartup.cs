using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Save;
using UnityEngine;
using Utils;
using World.TheCard;

namespace Globals
{
    public class HomeStartup : StartupBase
    {
        protected override async void Awake()
        {
            base.Awake();

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
            await UniTask.WaitUntil(() => Global.Instance != null);
            await Global.Instance.Init();
            await Global.Instance.WaitForInit<SoundManager>();
            Global.Instance.Get<SoundManager>().PlaySoundLoop("BGM_Home", 1);
            await FinishStartup();
            var setting = await new SaveManager().Load<SaveSettingGraphicModel>();
            Application.targetFrameRate = setting.Fps;
        }
    }
}