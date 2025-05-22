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
    public class PopupCharacterCardLineup : MonoBehaviour, ITabSwitcherWindow
    {
        [SerializeField] private TabSwitcher _tabSwitcherTeam;
        [SerializeField] private List<LineupCard> _lineupCards;
        [SerializeField] private HorizontalLayoutGroup _layoutGroupLineupCards;
        [SerializeField] private RectTransform _layoutTransformLineupCards;
        CancellationTokenSource _ctsLayoutGrLineUpCardSpearAnimation;

        private void Start()
        {
            _tabSwitcherTeam.OnTabSwitched += OnSwitchLineupTeam;
        }
        public async UniTask Init()
        {
            Vector2 pos = _layoutTransformLineupCards.anchoredPosition;
            pos.y = _layoutTransformLineupCards.rect.height + 50; // targetY là giá trị mới mày muốn đặt
            _layoutTransformLineupCards.anchoredPosition = pos;
            _tabSwitcherTeam.Init();
            _tabSwitcherTeam.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(true));
            InitLineupTeamTab();
            InitCards(1).Forget();
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
                await Spacing(-169.54f, 0);
                // var tasks = new List<UniTask>();
                // tasks.Add(Move(0));
                // tasks.Add(UniTask.WaitForSeconds(0.2f, cancellationToken: _ctsLayoutGrLineUpCardSpearAnimation.Token));
                // tasks.Add(Spacing(60));
                // await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsLayoutGrLineUpCardSpearAnimation.Token);

                await Move(0);
                await Spacing(60);
            }
            else
            {
                await Spacing(-169.54f);
                await Move(_layoutTransformLineupCards.rect.height + 50);
            }

            async UniTask Move(float targetY, float duration = 0.4f)
            {
                // Tính target Y dựa trên chiều cao layout
                // float height = _layoutTransformLineupCards.rect.height;
                //float targetY = isShow ? 0 : height + 50f; // bay lên luôn ngoài khung, thêm 50 cho chắc

                try
                {
                    await _layoutTransformLineupCards.DOAnchorPosY(targetY, duration)
                        .SetEase(Ease.OutQuad)
                        .WithCancellation(_ctsLayoutGrLineUpCardSpearAnimation.Token);
                }
                catch (OperationCanceledException)
                {
                }
            }

            async UniTask Spacing(float targetSpacing, float duration = 0.4f)
            {
                // Spacing tween
                //float targetSpacing = isShow ? 60f : -169.54f;

                try
                {
                    await DOTween.To(() => _layoutGroupLineupCards.spacing,
                            x => { _layoutGroupLineupCards.spacing = x; }, targetSpacing, duration)
                        .SetEase(Ease.OutCubic)
                        .WithCancellation(_ctsLayoutGrLineUpCardSpearAnimation.Token);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async UniTask InitCards(int teamLineupIndex)
        {
            _tabSwitcherTeam.Tabs.ForEach(i => i.TabSwitcherButton.SetButtonActive(false));
            await PlayLayoutGroupLineupCardsSpearAnimation(false);
            Debug.Log("Init lineup " + teamLineupIndex);
            var tasks = new List<UniTask>();
            for (int i = 0; i < _lineupCards.Count; i++)
            {
                tasks.Add(_lineupCards[i].Setup((byte)(i + 1), teamLineupIndex));
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ctsLayoutGrLineUpCardSpearAnimation.Token);
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