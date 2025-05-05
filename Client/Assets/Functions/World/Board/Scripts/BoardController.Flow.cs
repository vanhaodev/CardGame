using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using World.TheCard;

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
            _actionTurnManager.SetupOrders(_board.GetAllFactions());
            _actionTurnManager.MaxRoundCount = 99;

            _ctsBattleFlow?.Cancel();
            _ctsBattleFlow = new CancellationTokenSource();
            StartTurn();
        }

        /// <summary>
        /// Bắt đầu một trận đấu tại đây, sẽ thiết lập lần đầu những thứ cần thiết
        /// </summary>
        public async UniTask StartTurn()
        {
            _board.SetRound(_actionTurnManager.CurrentRoundCount, _actionTurnManager.MaxRoundCount);
            var battler = await TakeBattlerOfCurrentTurn(_ctsBattleFlow);
            if (battler.boardFactionPosition.Card.Battle.FactionIndex == 1) //Player
            {
                //show player turn UI
                ShowPlayerInput(battler.boardFactionPosition.Card);
            }
            else //AI
            {
                //========================[Chọn mục tiêu ngẫu nhiên]========================
                var targets = GetRandomTargets(battler.actionTurnActorModel.Card);
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
            _board.SetPlayerInput(true, currentTurnCard);
        }

        [Button]
        public async void OnPlayerInput()
        {
            var currentTurnBattler = _actionTurnManager.GetCurrentTurn();
            if (currentTurnBattler.Card.Battle.FactionIndex != 1) return;
            var targets = GetRandomTargets(currentTurnBattler.Card);
            _board.SetPlayerInput(false, null);
            if (targets.Count > 0)
            {
                await HandleBattlerAction(
                    currentTurnBattler,
                    _board.GetFactionByIndex(currentTurnBattler.Card.Battle.FactionIndex)
                        .GetPositionByIndex(currentTurnBattler.Card.Battle.MemberIndex),
                    targets,
                    _ctsBattleFlow
                );
            }

            StartTurn();
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