using System;
using System.Collections.Generic;
using Globals;
using Newtonsoft.Json;
using Popups;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using World.Board;
using Random = UnityEngine.Random;

namespace World.TheCard
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardModel _cardModel;
        [SerializeField] private Image _spriteFrame;
        [SerializeField] private Image _spriteCharacter;
        [SerializeField] private TextMeshProUGUI _txLevel;

        /// <summary>
        /// 5stars
        /// </summary>
        [SerializeField] private CardStarUI _cardStarUI;

        [SerializeField] CardEffect _effect;

        //============================[EVENT]==============================\\
        private readonly Subject<Card> _eventOnTouch = new Subject<Card>();
        public void InvokeEventOnTouch() => _eventOnTouch.OnNext(this);
        private IDisposable _onTouchListener;

        public void ListenEventOnTouch(Action<Card> action)
        {
            _onTouchListener?.Dispose();
            _onTouchListener = _eventOnTouch.Subscribe(action).AddTo(this);
        }
        
        //============================[]==============================\\


        public CardModel CardModel
        {
            get => _cardModel;
            set
            {
                _cardModel = value;
                Init();
            }
        }

        public Sprite SpriteCharacter => _spriteCharacter.sprite;


        private async void Init()
        {
            _spriteCharacter.sprite = await Global.Instance.Get<GameConfigs.GameConfig>().GetCardSprite(_cardModel);

            if (_cardStarUI.gameObject.activeSelf)
            {
                _cardStarUI.Set(_cardModel);
            }

            if (_txLevel.transform.parent.gameObject.activeSelf)
            {
                _txLevel.text = _cardModel.Level.GetLevel().ToString() ?? "1";
            }
        }

        /// <summary>
        /// if not use skill or attack target => Show this card's info <br/>
        /// if use skill, heal, buff or attack command => this will be the taraget
        /// </summary>
        public void OnTouch()
        {
            InvokeEventOnTouch();
        }

        public void OnHold()
        {

        }
    }
}