using UnityEngine;
using UnityEngine.UI;

namespace World.Card
{
    public class CardVital : MonoBehaviour
    {
        [SerializeField] private Image _spriteHpFill;
        [SerializeField] private Image _spriteMpFill;
        public void Show(bool isShow = true)
        {
            gameObject.SetActive(isShow);
        }
        public void UpdateHp(int hp, int hpMax)
        {
            _spriteHpFill.fillAmount = (float)hp / hpMax;
        }

        public void UpdateMp(int mp, int mpMax)
        {
            _spriteMpFill.fillAmount = (float)mp / mpMax;
        }
    }
}