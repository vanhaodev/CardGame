using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Globals
{
    public class GameStartup : SingletonMonoBehavior<GameStartup>
    {
        [SerializeField] GameObject[] _objEnableAfterDone;

        protected override void Awake()
        {
            base.Awake();
            StartupLocal();
            FinishStartup();
        }

        private async void StartupLocal()
        {
            await UniTask.WaitUntil(() =>
                GlobalConfig.Instance != null && GlobalTemplate.Instance != null && GlobalFunction.Instance != null &&
                GlobalCanvas.Instance != null);

            await InitFunction();
            LoadLocalSaveData();
            GlobalFunction.Instance.SoundManager.PlaySoundLoop(1, 1);

            ConnectNetwork();
        }

        private async UniTask InitFunction()
        {
            await GlobalConfig.Instance.Init();
            await GlobalTemplate.Instance.Init();
            await GlobalFunction.Instance.Init();
        }

        private void LoadLocalSaveData()
        {
            var save = new SaveManager();
            // GlobalConfig.Instance.Load(save.Load<SaveConfigModel>().Config);
            // GlobalTemplate.Instance.Load(save.Load<SaveTemplateModel>().Template);
            //set volume
            var sound = save.Load<SaveSettingSoundModel>();
            GlobalFunction.Instance.SoundManager.SetVolumeAll(SoundType.BackgroundMusic, sound.MusicVolume);
            GlobalFunction.Instance.SoundManager.SetVolumeAll(SoundType.Enviroment, sound.EnviromentVolume);
            GlobalFunction.Instance.SoundManager.SetVolumeAll(SoundType.Effect, sound.EffectVolume);
        }

        /// <summary>
        /// Check network and connect
        /// </summary>
        private async void ConnectNetwork()
        {
        }

        private void CheckVersion()
        {
        }

        public void HandleVersion()
        {
            FinishStartup();
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