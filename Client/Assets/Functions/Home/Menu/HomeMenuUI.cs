using System;
using DG.Tweening;
using UnityEngine;

public class HomeMenuUI : MonoBehaviour
{
    public DOTweenAnimation _tweenTapToPlay;
    public DOTweenAnimation _tweenPlayMenu;

    private void Start()
    {
        if (_tweenTapToPlay.tween == null) _tweenTapToPlay.CreateTween();
        _tweenTapToPlay.DOPlay();
    }

    public void TapTopPlay()
    {
        if (_tweenPlayMenu.tween == null) _tweenPlayMenu.CreateTween();
        _tweenTapToPlay.DOPlayBackwards();
        _tweenPlayMenu.DOPlay();
        
        _tweenTapToPlay.tween.OnComplete(() => { _tweenTapToPlay.tween.Kill(); }); 
        _tweenPlayMenu.tween.OnComplete(() => { _tweenPlayMenu.tween.Kill(); }); 
        // _tweenTapToPlay.tween.Kill();
        // _tweenPlayMenu.tween.Kill();
    }

}