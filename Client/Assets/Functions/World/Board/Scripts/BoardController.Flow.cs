using Newtonsoft.Json;
using UnityEngine;
using World.TheCard;

namespace World.Board
{
    public partial class BoardController : MonoBehaviour
    {
        /// <summary>
        /// Bắt đầu một trận đấu tại đây, sẽ thiết lập lần đầu những thứ cần thiết
        /// </summary>
        public void StartBattle()
        {
        }

        /// <summary>
        /// Mỗi round sẽ cần kiểm tra xem trạng thái tận đấu như thế nào, có khả dụng để sang round tiếp không hay phải kết thúc và thông báo kết quả
        /// </summary>
        public void SetupRound()
        {
            if (RoundIsLimit())
            {
                HandleGameDraw();
                return;
            }
        }

        /// <summary>
        /// Lấy tham chiếu của battler ở turn hiện tại
        /// </summary>
        public Card TakeBattlerOfCurrentTurn()
        {
            return null;
        }

        /// <summary>
        /// Xử lý hành động mà batter input, attack, buff....
        /// </summary>
        public void HandleBattlerAction()
        {
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