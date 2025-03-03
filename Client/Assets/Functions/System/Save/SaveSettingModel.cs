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
    public override void SetDefault()
    {
        base.SetDefault();
        Fps = -1; //unlimited
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
    public override void SetDefault()
    {
        base.SetDefault();
        MusicVolume = 1;
        EnviromentVolume = 1;
        EffectVolume = 1;
    }
}