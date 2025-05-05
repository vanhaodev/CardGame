using Cysharp.Threading.Tasks;
using Globals;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Popup
{
    public class PopupSetting : Popup
    {
        //side 1
        [SerializeField] Slider _sliderMusicVolume;
        [SerializeField] TextMeshProUGUI _txMusicVolume;
        [SerializeField] Slider _sliderEnviromentVolume;
        [SerializeField] TextMeshProUGUI _txEnviromentVolume;
        [SerializeField] Slider _sliderEffectVolume;
        [SerializeField] TextMeshProUGUI _txEffectVolume;
        [SerializeField] StepSlider _sliderFPS;
        [SerializeField] TextMeshProUGUI _txFPS;
        [SerializeField] StepSlider _sliderOtherPlayerEnableAround;
        [SerializeField] TextMeshProUGUI _txOtherPlayerEnableAround;

        //side 2
        [SerializeField] Toggle _toggleHideOtherPlayerTextures;
        [SerializeField] Toggle _toggleHideSkillEffects;
        [SerializeField] Toggle _toggleHideEnviromentEffects;

        //
        SettingController _controller;

        private void OnEnable()
        {
            _sliderMusicVolume.onValueChanged.AddListener((value) =>
            {
                _txMusicVolume.text = $"{value * 100:F0}%";
                Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.BGM, value);
            });
            _sliderEnviromentVolume.onValueChanged.AddListener((value) =>
            {
                _txEnviromentVolume.text = $"{value * 100:F0}%";
                Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.ENV, value);
            });
            _sliderEffectVolume.onValueChanged.AddListener((value) =>
            {
                _txEffectVolume.text = $"{value * 100:F0}%";
                Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.FX, value);
            });
            _sliderFPS.AddListener((value) =>
            {
                if (value == -1)
                {
                    _txFPS.text = $"Không giới hạn";
                }
                else
                {
                    _txFPS.text = $"{value:F0} FPS";
                }
            });
            _sliderOtherPlayerEnableAround.AddListener((value) =>
            {
                if (value == -1)
                {
                    _txOtherPlayerEnableAround.text = $"Không giới hạn";
                }
                else
                {
                    _txOtherPlayerEnableAround.text = $"{value:F0}";
                }
            });
        }

        private void OnDisable()
        {
            _sliderMusicVolume.onValueChanged.RemoveAllListeners();
            _sliderEnviromentVolume.onValueChanged.RemoveAllListeners();
            _sliderEffectVolume.onValueChanged.RemoveAllListeners();
            _sliderFPS.RemoveAllListeners();
            _sliderOtherPlayerEnableAround.RemoveAllListeners();
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
            _sliderOtherPlayerEnableAround.SetStepValue(graphic.OtherPlayerEnableAround);

            _toggleHideOtherPlayerTextures.isOn = graphic.IsHideOtherPlayerTextures;
            _toggleHideOtherPlayerTextures.onValueChanged?.Invoke(graphic.IsHideOtherPlayerTextures);

            _toggleHideSkillEffects.isOn = graphic.IsHideSkillEffects;
            _toggleHideSkillEffects.onValueChanged?.Invoke(graphic.IsHideSkillEffects);

            _toggleHideEnviromentEffects.isOn = graphic.IsHideEnviromentEffects;
            _toggleHideEnviromentEffects.onValueChanged?.Invoke(graphic.IsHideEnviromentEffects);
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
                OtherPlayerEnableAround = (short)_sliderOtherPlayerEnableAround.GetStepValue(),
                IsHideOtherPlayerTextures = _toggleHideOtherPlayerTextures.isOn,
                IsHideSkillEffects = _toggleHideSkillEffects.isOn,
                IsHideEnviromentEffects = _toggleHideEnviromentEffects.isOn,
            };
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.BGM, sound.MusicVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.ENV, sound.EnviromentVolume);
            Global.Instance.Get<SoundManager>().SetVolumeAll(SoundType.FX, sound.EffectVolume);
            await _controller.SaveSound(sound);
            await _controller.SaveGraphic(graphic);
        }

        public void RestoreDefault()
        {
            var sound = _controller.RestoreSoundDefault();
            var graphic = _controller.RestoreGraphicDefault();
            Init(sound, graphic);
        }

        public override async void Close()
        {
            await Save();
            base.Close();
        }
    }
}