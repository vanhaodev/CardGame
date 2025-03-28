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
       
        [SerializeField] CardEffect _effect;
        [SerializeField] CardBattle _battle;
        public CardBattle Battle => _battle;

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
            // _battle.SetupBattle(this);
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

            _effect.PlayTouchEffect();
            Global.Instance.Get<PopupManager>().ShowCard(_cardModel);
        }
    }
}