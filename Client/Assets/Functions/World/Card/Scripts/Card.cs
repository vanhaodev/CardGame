using System;
using Globals;
using Popup;
using UnityEngine;
using UnityEngine.UI;
using World.Board;
using Random = UnityEngine.Random;

namespace World.Card
{
    public class Card : MonoBehaviour
    {

        [SerializeField] private CardModel _cardModel;
        [SerializeField] private Image _spriteFrame;
        [SerializeField] private Image _spriteCharacter;
        [SerializeField] private CardVital _vital;

        public CardModel CardModel
        {
            get => _cardModel;
            set
            {
                _cardModel = value;
                Init();
            }
        }
        private async void Init()
        {
            _spriteCharacter.sprite = await Global.Instance.Get<GameConfig.GameConfig>().GetCardSprite(_cardModel);
        }
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
            if (Global.Instance.Get<BoardCommander>().IsSelectingTarget())
            {
                
                return;
            }
            Global.Instance.Get<PopupManager>().ShowCard(_cardModel);
        }
    }
}