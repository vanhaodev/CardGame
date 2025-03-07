using Globals;
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

        /// <summary>
        /// if not use skill or attack target => Show this card's info <br/>
        /// if use skill, heal, buff or attack command => this will be the taraget
        /// </summary>
        public void OnTouch()
        {
            if (GlobalFunction.Instance.BoardCommander.IsSelectingTarget())
            {
                
                return;
            }
            GlobalFunction.Instance.PopupManager.ShowCard();
        }
    }
}