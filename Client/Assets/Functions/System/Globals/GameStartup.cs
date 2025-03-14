using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Save;
using UnityEngine;
using Utils;
using World.Player.Character;

namespace Globals
{
    public class GameStartup : SingletonMonoBehavior<GameStartup>
    {
        [SerializeField] GameObject[] _objEnableAfterDone;
        public async UniTask WaitGlobal() => await UniTask.WaitUntil(() =>
            Global.Instance != null);
        public async UniTask InitGlobal() => await Global.Instance.Init();
        public async UniTask LoadSaveData()
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
        public async UniTask FinishStartup()
        {
            //show the ui
            foreach (var obj in _objEnableAfterDone)
            {
                obj.SetActive(true);
            }

            Destroy(gameObject);
            Global.Instance.Get<SoundManager>().PlaySoundLoop(1, 1);
        }
    }
}