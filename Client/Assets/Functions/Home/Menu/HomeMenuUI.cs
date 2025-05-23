using System;
using System.Collections.Generic;
using System.Linq;
using System.SceneLoader;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Globals;
using Popups;
using Save;
using Startup;
using UnityEngine;

public class HomeMenuUI : MonoBehaviour
{
    public DOTweenAnimation _tweenTapToPlay;
    public DOTweenAnimation _tweenPlayMenu;
    [SerializeField] private GameObject _btnContinue;

    private void Start()
    {
        if (_tweenTapToPlay.tween == null)
        {
            _tweenTapToPlay.CreateTween();
        }

        _tweenTapToPlay.DOPlayForward();
    }

    public void TapTopPlay()
    {
        var isNewbie = PlayerPrefs.GetInt("IsNewbie") == 1;
        _btnContinue.SetActive(!isNewbie);

        if (_tweenTapToPlay.tween == null)
        {
            _tweenTapToPlay.CreateTween();
            _tweenTapToPlay.tween.OnComplete(() =>
            {
                _tweenTapToPlay.tween.Kill();
                _tweenTapToPlay.tween = null;
            });
        }

        if (_tweenPlayMenu.tween == null)
        {
            _tweenPlayMenu.CreateTween();
            _tweenPlayMenu.tween.OnComplete(() =>
            {
                _tweenPlayMenu.tween.Kill();
                _tweenPlayMenu.tween = null;
            });
        }

        _tweenTapToPlay.DOPlayBackwards();
        _tweenPlayMenu.DOPlay();
    }

    public void NewGame()
    {
        var save = new SaveManager();
        save.Delete<SavePlayerModel>();
        Global.Instance.Get<SceneLoader>().LoadScene(1,
            null
        );
    }

    public void ContinueGame()
    {
        // Global.Instance.Get<SceneLoader>().LoadScene(1, () => GameStartup.Instance.GetTasks());
        Application.OpenURL("https://www.youtube.com/@vanhaodev2001");
    }

    public void GameInfo()
    {
        Application.OpenURL("https://www.youtube.com/@vanhaodev2001");
    }

    public void Setting()
    {
        Global.Instance.Get<PopupManager>().ShowSetting();
    }

    public void Exit()
    {
        Application.Quit();
    }
}