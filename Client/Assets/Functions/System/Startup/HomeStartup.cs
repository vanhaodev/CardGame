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
            Application.targetFrameRate = 60;
            await UniTask.WaitUntil(()=> Global.Instance != null);
            await Global.Instance.Init();
            await Global.Instance.WaitForInit<SoundManager>();
            Global.Instance.Get<SoundManager>().PlaySoundLoop(5, 1);
            await FinishStartup();
        }
    }
}