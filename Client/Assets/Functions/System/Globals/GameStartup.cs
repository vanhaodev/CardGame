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

        public async UniTask InitGame()
        {
            StartupLocal();
            FinishStartup();
        }
        private async void StartupLocal()
        {
            await UniTask.WaitUntil(() =>
                Global.Instance != null);

            await InitFunction();
            LoadLocalSaveData();
            Global.Instance.Get<SoundManager>().PlaySoundLoop(1, 1);
            
        }

        private async UniTask InitFunction()
        {
            await Global.Instance.Init();
        }

        private async UniTask LoadLocalSaveData()
        {
            var save = new SaveManager();
            //set volume
            var sound = await save.Load<SaveSettingSoundModel>();
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.BackgroundMusic, sound.MusicVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.Enviroment, sound.EnviromentVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.Effect, sound.EffectVolume);
            
            //set playerdata
            var playerData = save.Load<SavePlayerModel>();
            Global.Instance.Get<CharacterData>().CharacterModel = playerData.CharacterModel;
        }
        
        public void FinishStartup()
        {
            //show the ui
            foreach (var obj in _objEnableAfterDone)
            {
                obj.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}