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
        [Button]
        /// <summary>
        /// Bắt đầu một trận đấu tại đây, sẽ thiết lập lần đầu những thứ cần thiết
        /// </summary>
        public async UniTask StartBattle()
        {
            SetupRound();
        }

        /// <summary>
        /// Mỗi round sẽ cần kiểm tra xem trạng thái tận đấu như thế nào, có khả dụng để sang round tiếp không hay phải kết thúc và thông báo kết quả
        /// </summary>
        public bool SetupRound()
        {
            if (RoundIsLimit())
            {
                HandleGameDraw();
                return false;
            }

            _board.SetRound(_actionTurnManager.CurrentRoundCount, _actionTurnManager.MaxRoundCount);
            return true;
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
                    _board.SetPlayerInput(turn.Card.Battle.FactionIndex == 1, turn.Card);
                    actor = actorFaction.GetPositionByIndex(turn.Card.Battle.MemberIndex);
                }

                isFound = true;
            }

            await _actionTurnManager.SetCurrentActorTurnUI(cts);
            return (turn, actor);
        }

        public List<BoardFactionPosition> GetRandomTargets(ActionTurnActorModel actionTurnActorModel, int count = 1)
        {
            var targetFaction =
                _board.GetFactionByIndex(actionTurnActorModel.Card.Battle.FactionIndex == 1 ? 2 : 1);
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

            actionTurnActorModel.ResetAP();
            _actionTurnManager.UpdateOrderIndex();
        }

        public void HandleGameWin(RoundResultModel result)
        {
            Debug.Log(JsonConvert.SerializeObject(result));
        }

        public void HandleGameLose(RoundResultModel result)
        {
            Debug.Log(JsonConvert.SerializeObject(result));
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

        /// <summary>
        /// Kiểm tra round đã hết
        /// </summary>
        /// <returns></returns>
        public bool RoundIsLimit()
        {
            return _actionTurnManager.IsRoundOver();
        }

        /// <summary>
        /// Kiểm tra có phải là lượt cuối không, nếu có sẽ chuyển round mới
        /// </summary>
        /// <returns></returns>
        public bool IsLastTurn()
        {
            return _actionTurnManager.IsLastTurn();
        }
    }
}