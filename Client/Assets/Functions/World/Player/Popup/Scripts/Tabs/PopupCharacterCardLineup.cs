using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Globals;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tab;
using World.Player.Character;

namespace World.Player.PopupCharacter
{
    public class PopupCharacterCardLineup : MonoBehaviour
    {
        [SerializeField] private TabSwitcher _tabSwitcherTeam;
        [SerializeField] private List<LineupCard> _lineupCards;
        [SerializeField] private HorizontalLayoutGroup _layoutGroupLineupCards;
        [SerializeField] private RectTransform _layoutTransformLineupCards;
        CancellationTokenSource _ctsLayoutGrLineUpCardSpearAnimation;

        private void OnEnable()
        {
            _tabSwitcherTeam.Init();
            InitLineupTeamTab();

            _tabSwitcherTeam.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(true));
            Vector2 pos = _layoutTransformLineupCards.anchoredPosition;
            pos.y = _layoutTransformLineupCards.rect.height + 50; // targetY là giá trị mới mày muốn đặt
            _layoutTransformLineupCards.anchoredPosition = pos;
            InitCards(1);
        }

        private void Start()
        {
            _tabSwitcherTeam.OnTabSwitched += OnSwitchLineupTeam;
        }

        private void InitLineupTeamTab()
        {
            var maxLineupTeamCount = Global.Instance.Get<CharacterData>().CharacterModel.MaxLineupTeamCount;

            for (int i = 0; i < _tabSwitcherTeam.Tabs.Count; i++)
            {
                var tab = _tabSwitcherTeam.Tabs[i];
                var button = tab.TabSwitcherButton as CardLineupTeamTab;

                bool isLocked = i >= maxLineupTeamCount; // index nhỏ hơn max thì mở khóa
                button.SetLock(isLocked);
            }
        }

        private async UniTask PlayLayoutGroupLineupCardsSpearAnimation(bool isShow)
        {
            _ctsLayoutGrLineUpCardSpearAnimation?.Cancel();
            _ctsLayoutGrLineUpCardSpearAnimation?.Dispose();
            _ctsLayoutGrLineUpCardSpearAnimation = new CancellationTokenSource();
            if (isShow)
            {
                await Spacing(-169.54f);
                await Move(0);
                await Spacing(60);
            }
            else
            {
                await Spacing(-169.54f);
                await Move(_layoutTransformLineupCards.rect.height + 50);
            }

            async UniTask Move(float targetY)
            {
                // Tính target Y dựa trên chiều cao layout
                // float height = _layoutTransformLineupCards.rect.height;
                //float targetY = isShow ? 0 : height + 50f; // bay lên luôn ngoài khung, thêm 50 cho chắc

                try
                {
                    await _layoutTransformLineupCards.DOAnchorPosY(targetY, 0.2f)
                        .SetEase(Ease.OutQuad)
                        .WithCancellation(_ctsLayoutGrLineUpCardSpearAnimation.Token);
                }
                catch (OperationCanceledException)
                {
                }
            }

            async UniTask Spacing(float targetSpacing)
            {
                // Spacing tween
                //float targetSpacing = isShow ? 60f : -169.54f;

                try
                {
                    await DOTween.To(() => _layoutGroupLineupCards.spacing,
                            x => { _layoutGroupLineupCards.spacing = x; }, targetSpacing, 0.2f)
                        .SetEase(Ease.OutCubic)
                        .WithCancellation(_ctsLayoutGrLineUpCardSpearAnimation.Token);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async void InitCards(int teamLineupIndex)
        {
            _tabSwitcherTeam.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(false));
            await PlayLayoutGroupLineupCardsSpearAnimation(false);
            Debug.Log("Init lineup " + teamLineupIndex);
            for (int i = 0; i < _lineupCards.Count; i++)
            {
                _lineupCards[i].Setup((byte)(i + 1), teamLineupIndex);
            }

            await PlayLayoutGroupLineupCardsSpearAnimation(true);
            _tabSwitcherTeam.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(true));
        }

        public void OnSwitchLineupTeam(int index)
        {
            var maxLineupTeamCount = Global.Instance.Get<CharacterData>().CharacterModel.MaxLineupTeamCount;
            if (index + 1 > maxLineupTeamCount)
            {
                Debug.Log($"Slot is lock so return to {index - 1}");
                _tabSwitcherTeam.SwitchTab(index - 1);
                return;
            }

            InitCards(index + 1);
        }

        private void OnDestroy()
        {
            _tabSwitcherTeam.OnTabSwitched -= OnSwitchLineupTeam;
        }
    }
}