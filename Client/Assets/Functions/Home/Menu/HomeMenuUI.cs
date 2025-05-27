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

    public async void TapTopPlay()
    {
        var appState = await new SaveManager().Load<SaveAppModel>();
        _btnContinue.SetActive(!appState.IsFirstPlay);

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

    public async void NewGame()
    {
        var save = new SaveManager();
        //xóa save cũ
        save.Delete<SavePlayerModel>();        
        save.Delete<SaveAppModel>();
        Global.Instance.Get<SceneLoader>().LoadScene(1,
            null
        );
    }

    public void ContinueGame()
    {
        Global.Instance.Get<SceneLoader>().LoadScene(1,
            null
        );
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