using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Save;
using UnityEngine;
using Utils;
using World.Player.Character;

namespace Globals
{
    public class GameStartup : StartupBase, IGlobal
    {
        [SerializeField] GameObject[] _objEnableAfterDone;

        private async UniTask WaitGlobal() => await UniTask.WaitUntil(() =>
            Global.Instance != null);

        private async UniTask InitGlobal() => await Global.Instance.Init();
        địt mẹ phải tách gamestart ra khỏi global vì gamestartup là cái xuất hiện đầu tiên nhé

        private async UniTask LoadSaveData()
        {
            var save = new SaveManager();
            //set volume
            var sound = await save.Load<SaveSettingSoundModel>();
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.BackgroundMusic, sound.MusicVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.Enviroment, sound.EnviromentVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.Effect, sound.EffectVolume);

            //set playerdata
            var playerData = await save.Load<SavePlayerModel>();
            Global.Instance.Get<CharacterData>().CharacterModel = playerData.CharacterModel;
        }

        private async UniTask FinishStartup()
        {
            //show the ui
            foreach (var obj in _objEnableAfterDone)
            {
                obj.SetActive(true);
            }

            Destroy(gameObject);
            Global.Instance.Get<SoundManager>().PlaySoundLoop(1, 1);
        }

        public async UniTask Init()
        {
            AddTask(WaitGlobal);
            AddTask(InitGlobal);
            AddTask(LoadSaveData);
            AddTask(FinishStartup);
        }
    }
}