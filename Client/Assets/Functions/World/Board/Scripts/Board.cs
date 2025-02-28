using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace World.Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] GameObject _prefabCard;
        [SerializeField] List<BoardFaction> _objFactions;
        [SerializeField] private TextMeshProUGUI _txTurnRemainingTimeSecond; //turn will have time limit
    }
}