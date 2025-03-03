using UnityEngine;
using UnityEngine.UI;

namespace World.Card
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image _spriteFrame;
        [SerializeField] private Image _spriteCharacter;
        [SerializeField] private CardVital _vital;

        public void ShowVital(bool isShow = true)
        {
            _vital.gameObject.SetActive(isShow);
        }
    }
}