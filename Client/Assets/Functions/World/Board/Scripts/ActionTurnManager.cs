using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using World.Card;

namespace World.Board
{
    [System.Serializable]
    public partial class ActionTurnManager
    {
        /// <summary>
        /// Lưu trữ tất cả battler đã đăng ký trận này
        /// </summary>
        [BoxGroup("Main")] public List<ActionTurnActorModel> OriginalOrders = new List<ActionTurnActorModel>();

        /// <summary>
        /// Lưu trữ các battler có khả năng hành động trong round hiện tại
        /// </summary>
        [BoxGroup("Main")] public List<ActionTurnActorModel> ActionAvailableOrders = new List<ActionTurnActorModel>();

        [BoxGroup("Main")] public int MaxRoundCount;

        /// <summary>
        /// số round hiện tại bằng max thì round đó sẽ là round cuối và draw
        /// </summary>
        [BoxGroup("Main")] public int CurrentRoundCount;

        [BoxGroup("Main")] public ActionTurnActorModel CurrentTurn;

        public ActionTurnManager()
        {
            CurrentRoundCount = 0;
        }

        /// <summary>
        /// Đưa tất cả battler vào danh sách turn để chuẩn bị cho trận chiến
        /// </summary>
        /// <param name="factions"></param>
        public void SetupOrders(List<BoardFaction> factions)
        {
            OriginalOrders = new List<ActionTurnActorModel>();
            int factionIndex = 1; // Bắt đầu từ 1

            foreach (var faction in factions)
            {
                foreach (var pos in faction.GetAllPositions())
                {
                    var memberIndex = pos.MemberIndex;
                    var cardModel = pos.Card.CardModel;

                    OriginalOrders.Add(new ActionTurnActorModel()
                    {
                        Card = pos.Card,
                        FactionIndex = factionIndex, // factionIndex đúng giá trị
                        MemberIndex = memberIndex,
                        AttackSpeed = cardModel.CalculatedAttributes
                            .Find(i => i.Type == AttributeType.AttackSpeed)?.Value ?? 0
                    });
                }

                factionIndex++; // Tăng factionIndex sau mỗi faction, HIỆN chỉ có 2
            }

            UpdateOrderIndex();
        }

        /// <summary>
        /// cập nhật thứ tự lượt và điểm hành động (AP) <br/>
        /// Gọi mỗi khi bắt đầu một round
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void UpdateOrderIndex()
        {
            if (OriginalOrders.Count == 0) return;
            if (PreviousIndex() != ActionAvailableOrders.Count - 1) return;
            //dùng while để đảm bảo có ít nhất một người được hành đồng trong round
            //trong trường hợp chưa có ít nhất 1 battler đủ action point thì cứ cộng tiếp cho đến khi đủ thì mới bắt đầu round
            //thằng design nên tính toán speed để tránh việc while quá lâu nhé
            int maxWhile = 0;
            while (true)
            {
                foreach (var order in OriginalOrders)
                {
                    order.AccumulateAP();
                }

                if (OriginalOrders.Any(i => i.IsAvailable()))
                {
                    break;
                }

                if (maxWhile >= 99)
                {
                    throw new IndexOutOfRangeException("The loop is too large.");
                }

                maxWhile++;
            }


            ActionAvailableOrders = OriginalOrders
                .Where(i => i.IsAvailable())
                .OrderByDescending(i => i.ActionPoint)
                .ToList();
            CurrentRoundCount++;
            UpdateNewRoundActorTurnUI();
            Debug.Log($"Round: {CurrentRoundCount}\n" +
                      $"Available: {ActionAvailableOrders.Count}");
        }

        /// <summary>
        /// lấy index trước đó, turn tính từ 1 nên lấy index của mảng sẽ ra trước đó <br/>
        /// không lấy turn hiện tại -1 vì không thể biết turn hiện tại là null thì sẽ lỗi
        /// </summary>
        /// <returns></returns>
        public int PreviousIndex() => (CurrentTurn == null) ? 0 : ActionAvailableOrders.IndexOf(CurrentTurn);

        /// <summary>
        /// Lấy battler của turn tiếp theo, trả về null khi không tìm được ai
        /// </summary>
        /// <returns></returns>
        public ActionTurnActorModel GetNextTurn()
        {
            if (ActionAvailableOrders.Count == 0) return null;
            int previousIndex = PreviousIndex();

            // Duyệt qua danh sách để tìm nhân vật có thể hành động
            for (int i = 1; i <= ActionAvailableOrders.Count; i++)
            {
                int nextIndex = (previousIndex + i) % ActionAvailableOrders.Count;
                CurrentTurn = ActionAvailableOrders[nextIndex];
                return CurrentTurn;
            }

            Debug.LogError($"Turn bị null\n" +
                           $"ActionAvailableOrders: {ActionAvailableOrders.Count} (chết: {ActionAvailableOrders.Count(i => !i.Card.Battle.IsDead)}");
            return null; // Không có ai có thể hành động
        }

        /// <summary>
        /// round kết thúc chưa phân thắng bại sẽ cho là hòa, đối với phó bản thì tính là chưa vượt qua được
        /// </summary>
        /// <returns></returns>
        public bool IsRoundOver()
        {
            return CurrentRoundCount >= MaxRoundCount;
        }
    }
}