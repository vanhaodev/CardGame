using System.SceneLoader;
using Cysharp.Threading.Tasks;
using Globals;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Popups
{
    public class PopupSetting : Popup
    {
        private string _titleMusic = "Music: {0}%";
        private string _titleEffect = "Effect: {0}%";
        private string _titleEnviroment = "Enviroment: {0}%";
        private string _titleFPS = "FPS: {0}";

        private string _titleOtherPlayerEnableAround = "Player: {0}";

        //side 1
        [SerializeField] Slider _sliderMusicVolume;
        [SerializeField] TextMeshProUGUI _txMusicVolume;
        [SerializeField] Slider _sliderEnviromentVolume;
        [SerializeField] TextMeshProUGUI _txEnviromentVolume;
        [SerializeField] Slider _sliderEffectVolume;
        [SerializeField] TextMeshProUGUI _txEffectVolume;
        [SerializeField] StepSlider _sliderFPS;
        [SerializeField] TextMeshProUGUI _txFPS;
        // [SerializeField] StepSlider _sliderOtherPlayerEnableAround;
        // [SerializeField] TextMeshProUGUI _txOtherPlayerEnableAround;

        //side 2
        // [SerializeField] Toggle _toggleHideOtherPlayerTextures;
        // [SerializeField] Toggle _toggleHideSkillEffects;
        // [SerializeField] Toggle _toggleHideEnviromentEffects;

        //bot button
        [SerializeField] Button _btnGoToHomeScene;
        //
        SettingController _controller;

        private void OnEnable()
        {
            _sliderMusicVolume.onValueChanged.AddListener((value) =>
            {
                _txMusicVolume.text = string.Format(_titleMusic, $"{value * 100:F0}");
                Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.BGM, value);
            });
            _sliderEnviromentVolume.onValueChanged.AddListener((value) =>
            {
                _txEnviromentVolume.text = string.Format(_titleEnviroment, $"{value * 100:F0}");
                Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.ENV, value);
            });
            _sliderEffectVolume.onValueChanged.AddListener((value) =>
            {
                _txEffectVolume.text = string.Format(_titleEffect, $"{value * 100:F0}");
                Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.FX, value);
            });
            _sliderFPS.AddListener((value) =>
            {
                if (value == -1)
                {
                    _txFPS.text = string.Format(_titleFPS, "Unlimited");
                }
                else
                {
                    _txFPS.text = string.Format(_titleFPS, value);
                }
            });
            // _sliderOtherPlayerEnableAround?.AddListener((value) =>
            // {
            //     if (value == -1)
            //     {
            //         _txOtherPlayerEnableAround.text = string.Format(_titleOtherPlayerEnableAround, "Unlimited");
            //     }
            //     else
            //     {
            //         _txOtherPlayerEnableAround.text = string.Format(_titleOtherPlayerEnableAround, $"{value:F0}");
            //     }
            // });

            if (SceneManager.GetActiveScene().buildIndex == 0) //không hiện ở home
            {
                _btnGoToHomeScene.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            _sliderMusicVolume.onValueChanged.RemoveAllListeners();
            _sliderEnviromentVolume.onValueChanged.RemoveAllListeners();
            _sliderEffectVolume.onValueChanged.RemoveAllListeners();
            _sliderFPS.RemoveAllListeners();
            // _sliderOtherPlayerEnableAround?.RemoveAllListeners();
        }

        private async void Start()
        {
            _controller = GetComponent<SettingController>();
            var sound = await _controller.LoadSound();
            var graphic = await _controller.LoadGraphic();
            Init(sound, graphic);
        }

        public void Init(SaveSettingSoundModel sound, SaveSettingGraphicModel graphic)
        {
            _sliderMusicVolume.value = sound.MusicVolume;
            _sliderMusicVolume.onValueChanged?.Invoke(sound.MusicVolume);

            _sliderEnviromentVolume.value = sound.EnviromentVolume;
            _sliderEnviromentVolume.onValueChanged?.Invoke(sound.EnviromentVolume);

            _sliderEffectVolume.value = sound.EffectVolume;
            _sliderEffectVolume.onValueChanged?.Invoke(sound.EffectVolume);

            _sliderFPS.SetStepValue(graphic.Fps);
            // _sliderOtherPlayerEnableAround.SetStepValue(graphic.OtherPlayerEnableAround);

            // _toggleHideOtherPlayerTextures.isOn = graphic.IsHideOtherPlayerTextures;
            // _toggleHideOtherPlayerTextures.onValueChanged?.Invoke(graphic.IsHideOtherPlayerTextures);
            //
            // _toggleHideSkillEffects.isOn = graphic.IsHideSkillEffects;
            // _toggleHideSkillEffects.onValueChanged?.Invoke(graphic.IsHideSkillEffects);
            //
            // _toggleHideEnviromentEffects.isOn = graphic.IsHideEnviromentEffects;
            // _toggleHideEnviromentEffects.onValueChanged?.Invoke(graphic.IsHideEnviromentEffects);
        }

        public async UniTask Save()
        {
            var sound = new SaveSettingSoundModel()
            {
                MusicVolume = _sliderMusicVolume.value,
                EnviromentVolume = _sliderEnviromentVolume.value,
                EffectVolume = _sliderEffectVolume.value
            };
            var graphic = new SaveSettingGraphicModel()
            {
                Fps = (short)_sliderFPS.GetStepValue(),
                // OtherPlayerEnableAround = (short)_sliderOtherPlayerEnableAround.GetStepValue(),
                // IsHideOtherPlayerTextures = _toggleHideOtherPlayerTextures.isOn,
                // IsHideSkillEffects = _toggleHideSkillEffects.isOn,
                // IsHideEnviromentEffects = _toggleHideEnviromentEffects.isOn,
            };
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.BGM, sound.MusicVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.ENV, sound.EnviromentVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.FX, sound.EffectVolume);
            await _controller.SaveSound(sound);
            await _controller.SaveGraphic(graphic);
        }

        public async UniTask RestoreDefault()
        {
            var sound = await _controller.RestoreSoundDefault();
            var graphic = await _controller.RestoreGraphicDefault();
            Init(sound, graphic);
        }

        public override async void Close(float fadeDuration = 0.3f)
        {
            await Save();
            base.Close(fadeDuration);
        }

        public void GoToHomeScene()
        {
            Global.Instance.Get<SceneLoader>().LoadScene(0, null);
        }
    }
}