using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using Utils;

namespace World.TheCard
{
    public class CardVital : MonoBehaviour
    {
        [SerializeField] private SmoothVitalFill _hpFill;
        [SerializeField] private SmoothVitalFill _upFill;
        
        public void Show(bool isShow = true)
        {
            gameObject.SetActive(isShow);
        }

        public void UpdateHp(int hp, int hpMax)
        {
           _hpFill.UpdateFill( hp, hpMax);
        }

        public void UpdateUp(int up)
        {
            _upFill.UpdateFill(up, 100);
        }
    }
}
