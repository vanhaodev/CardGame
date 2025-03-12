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
        
        private void Start()
        {
            //test
            _cardModel.TemplateId = (ushort)Random.Range(1, 3);
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
                _cardModel.CalculatedAttributes.Add(new AttributeModel
                {
                    Type = type,
                    Value = Random.Range(1, 500)
                });
            }
            //
            Init();
        }

        private async void Init()
        {
            var template = await Global.Instance.Get<CardLoader>().GetCardTemplate(_cardModel.TemplateId);
            _spriteCharacter.sprite = template.StarSkins[0];
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