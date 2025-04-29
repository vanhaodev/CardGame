using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("_skillPointUI")] [SerializeField] private SkillPointUI skillPointUIUI;
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
    }
}