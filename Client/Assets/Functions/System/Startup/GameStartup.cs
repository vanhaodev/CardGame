using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using Save;
using UnityEngine;
using Utils;
using World.Player.Character;

namespace Startup
{
    public class GameStartup : StartupBase
    {
        protected override void Awake()
        {
            base.Awake();
            AddTask(Global.Instance.Init);
            AddTask(FinishStartup);
            AddTask(async () =>
            {
                Global.Instance.Get<SoundManager>().PlaySoundLoop("BGM_Lobby", 1);
            });
            // Debug.Log("Initialized Game Startup");
        }
        public async UniTask Init()
        {
            
        }
    }
}