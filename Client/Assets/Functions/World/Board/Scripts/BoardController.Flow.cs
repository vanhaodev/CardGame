using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using World.TheCard;
using Random = UnityEngine.Random;

namespace World.Board
{
    public partial class BoardController : MonoBehaviour
    {
        private CancellationTokenSource _ctsBattleFlow;

        [Button]
        public async void SetupBattleFlow()
        {
            await SetupBoardCards();

            _board.GetFactionByIndex(1).ResetToOriginalPosition();
            _board.GetFactionByIndex(2).ResetToOriginalPosition();
            _board.SetSkill(null,
                _board.GetFactionByIndex(1).FactionAttributes[FactionAttributeType.SkillPoint]);
            _actionTurnManager.SetupOrders(_board.GetAllFactions());
            _actionTurnManager.MaxRoundCount = 99;
            _ctsBattleFlow?.Cancel();
            _ctsBattleFlow = new CancellationTokenSource();

            _targetSelectorController.Show(false);
            StartTurn();
        }

        /// <summary>
        /// Bắt đầu một trận đấu tại đây, sẽ thiết lập lần đầu những thứ cần thiết
        /// </summary>
        public async UniTask StartTurn()
        {
            _board.SetRound(_actionTurnManager.CurrentRoundCount, _actionTurnManager.MaxRoundCount);
            var battler = await TakeBattlerOfCurrentTurn(_ctsBattleFlow);
            if (battler.boardFactionPosition == null)
            {
                StartTurn();
                return;
            }
            _actionTurnManager.ShowTurnerMark(battler.boardFactionPosition.Card);
            var targets = GetRandomTargets(battler.actionTurnActorModel.Card);
            if (battler.boardFactionPosition.Card.Battle.FactionIndex == 1) //Player
            {
                //show player turn UI
                ShowPlayerInput(battler.boardFactionPosition.Card);
                _targetSelectorController.Show();
                //chọn giúp player trước, họ thể attack luôn cũng được
                _targetSelectorController.OnTouch(targets[0].Card);
            }
            else //AI
            {
                await UniTask.WaitForSeconds(1, cancellationToken: _ctsBattleFlow.Token); //tránh diễn ra quá nhanh
                //========================[Chọn mục tiêu ngẫu nhiên]========================
                if (targets.Count > 0)
                {
                    await HandleBattlerAction(
                        battler.actionTurnActorModel,
                        battler.boardFactionPosition,
                        targets,
                        _ctsBattleFlow
                    );
                    StartTurn();
                }
            }
        }

        public async void ShowPlayerInput(Card currentTurnCard)
        {
            //will load skill to input
            Debug.Log("ShowPlayerInput");
            _board.SetSkill(currentTurnCard,
                _board.GetFactionByIndex(currentTurnCard.Battle.FactionIndex)
                    .FactionAttributes[FactionAttributeType.SkillPoint]);
            _board.SetPlayerInput(true, currentTurnCard);
        }

        /// <summary>
        /// 1: basic | 2: advanced | 3: ultimate
        /// </summary>
        /// <param name="skillSlotIndex"></param>
        private async void OnPlayerSkill(int skillSlotIndex)
        {
            _targetSelectorController.Show(false);
            var currentTurnBattler = _actionTurnManager.GetCurrentTurn();
            if (currentTurnBattler.Card.Battle.FactionIndex != 1) return;
            var targets = _targetSelectorController.GetSelectingFactions();

            var faction = _board.GetFactionByIndex(currentTurnBattler.Card.Battle.FactionIndex);
            var actor = faction.GetPositionByIndex(currentTurnBattler.Card.Battle.MemberIndex);
            _board.SetPlayerInput(false, null);
            if (targets.Count > 0)
            {
                await HandleBattlerAction(
                    currentTurnBattler,
                    actor,
                    targets,
                    _ctsBattleFlow
                );
            }

            if (skillSlotIndex == 1)
            {
                faction.AddSkillPoint(1);
            }
            else if (skillSlotIndex == 2)
            {
                faction.AddSkillPoint(-1);
            }
            else if (skillSlotIndex == 3)
            {
                actor.Card.Battle.AddUltimatePoint(-100);
            }

            _board.SetSkill(actor.Card, faction.FactionAttributes[FactionAttributeType.SkillPoint]);
            StartTurn();
        }

        private void OnPlayerBasicAttack()
        {
            OnPlayerSkill(1);
        }

        private void OnPlayerAdvancedSkill()
        {
            OnPlayerSkill(2);
        }

        private void OnPlayerUltimateSkill()
        {
            OnPlayerSkill(3);
        }

        /// <summary>
        /// Lấy tham chiếu của battler ở turn hiện tại
        /// </summary>
        public async UniTask<(ActionTurnActorModel actionTurnActorModel, BoardFactionPosition boardFactionPosition)>
            TakeBattlerOfCurrentTurn(CancellationTokenSource cts)
        {
            bool isFound = false;
            ActionTurnActorModel turn = null;
            BoardFactionPosition actor = null;
            while (!isFound)
            {
                turn = _actionTurnManager.GetNextTurn();
                //chưa kịp hành động thì đã bị killed
                if (!turn.IsAvailable())
                {
                    //vì có tuner không còn khả năng hành động nên update lại danh sách cho clean
                    _actionTurnManager.UpdateOrderIndex();
                }
                else
                {
                    //Get actor
                    var actorFaction = _board.GetFactionByIndex(turn.Card.Battle.FactionIndex);
                    actor = actorFaction.GetPositionByIndex(turn.Card.Battle.MemberIndex);
                }

                isFound = true;
            }

          
            await _actionTurnManager.SetCurrentActorTurnUI(cts);
            return (turn, actor);
        }

        public List<BoardFactionPosition> GetRandomTargets(Card card, int count = 1)
        {
            var targetFaction =
                _board.GetFactionByIndex(card.Battle.FactionIndex == 1 ? 2 : 1);
            var validTargetIndexes = GetTargets(targetFaction);

            if (validTargetIndexes.Count == 0)
                return new List<BoardFactionPosition>();

            var randomTargetIndexes = validTargetIndexes
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(count)
                .ToList();

            return randomTargetIndexes
                .Select(index => targetFaction.GetPositionByIndex(index))
                .ToList();
        }

        /// <summary>
        /// Xử lý hành động mà batter input, attack, buff....
        /// </summary>
        public async UniTask HandleBattlerAction(ActionTurnActorModel actionTurnActorModel, BoardFactionPosition actor,
            List<BoardFactionPosition> targets,
            CancellationTokenSource cts)
        {
            _actionTurnManager.ShowTurnerMark(null);
            if (Random.Range(0, 2) == 1)
            {
                await Ranged(actor, targets, cts);
            }
            else
            {
                await Melee(actor, targets, cts);
            }

            await PerformCameraReset(_ctsBattleFlow);
            await UniTask.Yield(cancellationToken: _ctsBattleFlow.Token);
            actionTurnActorModel.ResetAP();
            _actionTurnManager.UpdateOrderIndex();

            var roundResult = GetRoundResult();
            if (roundResult.WinFactionIndex == 1)
            {
                HandleGameWin();
                return;
            }
            else if (roundResult.WinFactionIndex == 2)
            {
                HandleGameLose();
                return;
            }
            else if (roundResult.WinFactionIndex == 3)
            {
                HandleGameDraw();
                return;
            }

            if (_actionTurnManager.IsRoundOver())
            {
                HandleGameDraw();
                return;
            }
        }

        public void HandleGameWin()
        {
            Debug.Log("WIN");
        }

        public void HandleGameLose()
        {
            Debug.Log("LOSE");
        }

        public void HandleGameDraw()
        {
            Debug.Log("Draw");
        }

        /// <summary>
        /// Mỗi round kết thúc thì sẽ kiểm tra xem có phe nào chết không, nếu có thì sẽ xử lý thắng thua để kết thúc trận
        /// </summary>
        /// <returns></returns>
        public RoundResultModel GetRoundResult()
        {
            bool faction1Dead = _board.GetFactionByIndex(1).IsAllDead();
            bool faction2Dead = _board.GetFactionByIndex(2).IsAllDead();

            if (faction1Dead && faction2Dead)
                return new RoundResultModel { WinFactionIndex = 3 }; // Hòa
            if (faction1Dead)
                return new RoundResultModel { WinFactionIndex = 2 }; // Phe 2 thắng
            if (faction2Dead)
                return new RoundResultModel { WinFactionIndex = 1 }; // Phe 1 thắng

            return new RoundResultModel { WinFactionIndex = 0 }; // Nothing;
        }
    }
}