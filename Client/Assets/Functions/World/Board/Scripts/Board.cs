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
        public Button.ButtonClickedEvent BtnBasicAttackOnClick => _btnBasicAttack.Button.onClick;
        public Button.ButtonClickedEvent BtnAdvancedSkillOnClick => _btnAdvancedSkill.Button.onClick;
        public Button.ButtonClickedEvent BtnUltimateSkillOnClick => _btnUltimateSkill.Button.onClick;
        [SerializeField] private DOTweenAnimation[] _tweenPlayerInputUIs;

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
            for (int i = 0; i < _tweenPlayerInputUIs.Length; i++)
            {
                if (_tweenPlayerInputUIs[i].tween == null || !_tweenPlayerInputUIs[i].tween.IsActive())
                {
                    _tweenPlayerInputUIs[i].CreateTween();
                }

                // Gán lại hành vi khi tween hoàn tất
                _tweenPlayerInputUIs[i].tween.OnComplete(() =>
                {
                    _btnBasicAttack.Button.enabled = isShow;
                    _btnAdvancedSkill.Button.enabled = isShow;
                    _btnUltimateSkill.Button.enabled = isShow;
                });

                if (isShow)
                {
                    _tweenPlayerInputUIs[i].tween.PlayForward();
                }
                else
                {
                    _tweenPlayerInputUIs[i].tween.PlayBackwards();
                }
            }
        }

        public void SetSkill(Card card, int skillPoint)
        {
            _skillPointUI.SetPoint(skillPoint);
            _btnBasicAttack.SetSkillUsable(100);
            _btnAdvancedSkill.SetSkillUsable(skillPoint > 0 ? 100 : 0);
            _btnUltimateSkill.SetSkillUsable(card.Battle.UltimatePoint);
        }

        private void OnDisable()
        {
            for (int i = 0; i < _tweenPlayerInputUIs.Length; i++)
            {
                if (_tweenPlayerInputUIs[i].tween != null)
                {
                    _tweenPlayerInputUIs[i].tween.Kill();
                    _tweenPlayerInputUIs[i].tween = null; // Optional
                }
            }
        }
    }
}