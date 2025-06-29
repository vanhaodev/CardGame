using Cysharp.Threading.Tasks;

namespace Save
{
    [System.Serializable]
    public class SaveSettingGraphicModel : SaveModel
    {
        /// <summary>
        /// Khung hình trên giây
        /// </summary>
        public short Fps;

        public short OtherPlayerEnableAround;
        public bool IsHideOtherPlayerTextures;
        public bool IsHideSkillEffects;
        public bool IsHideEnviromentEffects;

        public SaveSettingGraphicModel()
        {
            DataName = "Graphic";
        }
        public override async UniTask SetDefault()
        {
            await base.SetDefault();
            Fps = 60; //unlimited
            OtherPlayerEnableAround = -1; //unlimited
            IsHideOtherPlayerTextures = false;
            IsHideSkillEffects = false;
            IsHideEnviromentEffects = false;
        }
    }

    [System.Serializable]
    public class SaveSettingSoundModel : SaveModel
    {
        public float MusicVolume;
        public float EnviromentVolume;
        public float EffectVolume;

        public SaveSettingSoundModel()
        {
            DataName = "Sound";
        }
        public override async UniTask SetDefault()
        {
            await base.SetDefault();
            MusicVolume = 1;
            EnviromentVolume = 1;
            EffectVolume = 1;
        }
    }
}