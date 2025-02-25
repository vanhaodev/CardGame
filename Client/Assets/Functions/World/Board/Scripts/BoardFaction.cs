using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Board
{
    public class BoardFaction
    {
        [SerializeField] private TextMeshProUGUI _txRemainingTimeSecond; //turn will have time limit
        [SerializeField] private Image _imageFrame; //skin just apply for owner faction
    }
}