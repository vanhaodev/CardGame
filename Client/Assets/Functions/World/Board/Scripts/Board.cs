using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace World.Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] GameObject _prefabCard;
        [SerializeField] List<BoardFaction> _factions;
        [SerializeField] private TextMeshProUGUI _txTurnRemainingTimeSecond; //turn will have time limit
        public List<BoardFaction> GetFactions() => _factions;
        public BoardFaction GetFaction(int index) => _factions[index-1];
    }
}