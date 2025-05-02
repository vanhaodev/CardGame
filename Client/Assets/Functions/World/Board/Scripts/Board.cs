using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using World.TheCard;

namespace World.Board
{
    public class Board : MonoBehaviour
    {
        //----------------- Display ----------------\\
        [SerializeField] Canvas _canvas;

        /// <summary>
        /// đại loại là nền cho trận chiến sẽ thay đổi phụ thuộc vào khu vực đang chiến đấu, tăng độ nhận diện
        /// </summary>
        [SerializeField] private Image _boardBackground;

        [SerializeField] private TextMeshProUGUI _txRound;

        [SerializeField] private TextMeshProUGUI _txTurnTimeCountdown;

        [SerializeField] private SkillButton _btnBasicAttack;
        [SerializeField] private SkillButton _btnAdvancedSkill;
        [SerializeField] private SkillButton _btnUltimateSkill;
        [SerializeField] private SkillPointUI _skillPointUI;

        [SerializeField] private DOTweenAnimation _tweenPlayerInputButtons;

        //----------------- Entity ----------------\\
        [SerializeField] GameObject _prefabCard;
        [SerializeField] List<BoardFaction> _factions;
        public List<BoardFaction> GetAllFactions() => _factions;
        public BoardFaction GetFactionByIndex(int index) => _factions[index - 1];

        private void Start()
        {
            _txRound.text = String.Empty;
            _txTurnTimeCountdown.text = String.Empty;
        }

        public void SetRound(int currentRoundCount, int maxRoundCount)
        {
            _txRound.text = $"{currentRoundCount}/{maxRoundCount}";
        }

        public void SetTurnCountDown(float turnCountDown)
        {
            _txTurnTimeCountdown.text = turnCountDown.ToString("0");
        }

        [Button]
        public void SetPlayerInput(bool isShow, Card currentTurnCard)
        {
            // Nếu tween null hoặc đã bị Kill, thì tạo lại
            if (_tweenPlayerInputButtons.tween == null || !_tweenPlayerInputButtons.tween.IsActive())
            {
                _tweenPlayerInputButtons.CreateTween();
            }

            _tweenPlayerInputButtons.tween.OnComplete(() =>
            {
                _btnBasicAttack.Button.enabled = isShow;
                _btnAdvancedSkill.Button.enabled = isShow;
                _btnUltimateSkill.Button.enabled = isShow;
            });
            if (isShow)
            {
                _tweenPlayerInputButtons.tween.PlayForward();
            }
            else
            {
                _tweenPlayerInputButtons.tween.PlayBackwards();
            }
        }

        private void OnDisable()
        {
            _tweenPlayerInputButtons.tween.Kill();
            _tweenPlayerInputButtons.tween = null; // Optional
        }
    }
}