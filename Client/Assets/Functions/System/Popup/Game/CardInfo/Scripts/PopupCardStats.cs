using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Functions.World.Player;
using GameConfigs;
using Globals;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Utils.Tab;
using World.Player.Character;
using World.Player.PopupCharacter;
using World.TheCard;

namespace Popups
{
    public class PopupCardStats : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] Card _card;
        [SerializeField] RectTransform _rectTransformContent;
        [SerializeField] TextMeshProUGUI _txCardName;

        //====================[Element & Class]=====================\\
        [SerializeField] private CardElementUI _element;

        [SerializeField] private CardClassUI _class;

        //====================[Level]=====================\\
        [SerializeField] TextMeshProUGUI _txLevel;
        [SerializeField] private TextMeshProUGUI _txLevelExp;
        [SerializeField] private Image _imgLevelExpFill;

        [SerializeField] CardAttributeUI _cardAttributeUI;

        //====================[Feature]=================//
        [SerializeField] private Button _btnAddToLineup;
        [SerializeField] private Button _btnRemoveFromLineup;

        public async UniTask Init(TabSwitcherWindowModel model = null)
        {
            var tabSwitcherWindowModel = model as PopupCardTabSwitcherWindowModel;
            var cardModel = tabSwitcherWindowModel.PopupCardModel.CardModel;
            _card.CardModel = cardModel;
            var template = await Global.Instance.Get<GameConfig>().GetCardTemplate(cardModel.TemplateId);

            //====================[Name]=================\\
            var starColor = Global.Instance.Get<GameConfig>().GetRarityColor(cardModel.Star);
            // Gán text như bình thường (không dùng richtext)
            _txCardName.text = template.Name;
            _txCardName.enableVertexGradient = true;
            ColorUtility.TryParseHtmlString(starColor.gradient, out var topColor);
            ColorUtility.TryParseHtmlString(starColor.gradient2, out var bottomColor);
            _txCardName.colorGradient = new VertexGradient(
                topColor, topColor, // Top Left, Top Right
                bottomColor, bottomColor // Bottom Left, Bottom Right
            );

            _element.Init(template.Element);
            _class.Init(template.Class);
            //
            _txLevel.text = cardModel.Level.GetLevel().ToString();
            _txLevelExp.text =
                $"{cardModel.Level.GetExpCurrent(false).ToString()}" +
                "<color=black>/</color>" +
                $"{cardModel.Level.GetExpNext(false).ToString()}";
            _imgLevelExpFill.fillAmount = cardModel.Level.GetProgress(false) / 100;

            _cardAttributeUI.Init( AttributeModel.ToDictionary(cardModel.CalculatedAttributes));

            //=============[BTN]=================\\
            _btnAddToLineup.gameObject.SetActive(tabSwitcherWindowModel.PopupCardModel is PopupCardEquipModel);
            _btnRemoveFromLineup.gameObject.SetActive(tabSwitcherWindowModel.PopupCardModel is PopupCardUnequipModel);
            if (_btnAddToLineup.gameObject.activeSelf)
            {
                _btnAddToLineup.onClick.RemoveAllListeners();
                _btnAddToLineup.onClick.AddListener(() =>
                    EquipToLineup(tabSwitcherWindowModel.PopupCardModel as PopupCardEquipModel, cardModel,
                        tabSwitcherWindowModel.OnClosePopupCard));
            }

            if (_btnRemoveFromLineup.gameObject.activeSelf)
            {
                _btnRemoveFromLineup.onClick.RemoveAllListeners();
                _btnRemoveFromLineup.onClick.AddListener(() =>
                    UnequipFromLineup(tabSwitcherWindowModel.PopupCardModel as PopupCardUnequipModel, cardModel,
                        tabSwitcherWindowModel.OnClosePopupCard));
            }

            switch (tabSwitcherWindowModel.PopupCardModel)
            {
                case PopupCardEquipModel equip:
                {
                    break;
                }
                case PopupCardUnequipModel unequip:
                {
                    break;
                }
                case PopupCardCollectionModel collection:
                {
                    break;
                }
                case PopupCardBattleModel battle:
                {
                    break;
                }
            }
        }

        public async UniTask LateInit()
        {
            await _cardAttributeUI.RefreshUI();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransformContent);
        }

        private void EquipToLineup(PopupCardEquipModel equip, CardModel cardModel, Action onClosePopup)
        {
            var charData = Global.Instance.Get<CharacterData>();
            byte maxLineupCount = charData.CharacterModel.MaxLineupTeamCount;
            var lineups = charData.CharacterModel.CardLineups;
            if (equip.LineupIndex >= maxLineupCount)
            {
                Debug.LogError($"LineupIndex {equip.LineupIndex} vượt quá MaxLineupTeamCount {maxLineupCount}");
                return;
            }

            //Tạo lineup list cho đủ
            while (lineups.Count <= equip.LineupIndex)
            {
                lineups.Add(new CardLineupModel());
            }

            if (lineups[equip.LineupIndex].Cards == null)
                lineups[equip.LineupIndex].Cards = new Dictionary<byte, int>();

            lineups[equip.LineupIndex].Cards[equip.SlotIndex] = cardModel.Id;

            Debug.Log($"Equip {cardModel.Id} to slot {equip.SlotIndex} of lineup {equip.LineupIndex}");
            //==========================================================//
            charData.CharacterModel.InvokeOnCardCollectionChanged();
            charData.CharacterModel.InvokeOnCardLineupChanged();
            onClosePopup?.Invoke();
        }

        private void UnequipFromLineup(PopupCardUnequipModel unequip, CardModel cardModel, Action onClosePopup)
        {
            Debug.Log(JsonConvert.SerializeObject(unequip));
            var charData = Global.Instance.Get<CharacterData>();
            if (unequip.LineupIndex < 0 || unequip.LineupIndex >= charData.CharacterModel.CardLineups.Count)
            {
                Debug.LogError("Lineup không tồn tại hehe");
                return;
            }

            var cards = charData.CharacterModel.CardLineups[unequip.LineupIndex].Cards;
            if (cards == null)
            {
                Debug.LogError("Cards chưa được khởi tạo");
                return;
            }

            bool removed = cards.Remove((byte)unequip.SlotIndex);
            if (!removed)
            {
                Debug.LogError("Card không có trong lineup");
                return;
            }

            Debug.Log($"Unequip {cardModel.Id} from slot {unequip.SlotIndex} of lineup {unequip.LineupIndex}");
            //==========================================================//
            charData.CharacterModel.InvokeOnCardCollectionChanged();
            charData.CharacterModel.InvokeOnCardLineupChanged();
            onClosePopup?.Invoke();
        }
    }
}