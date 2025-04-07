using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using World.Card;

namespace World.Board
{
    [System.Serializable]
    public class ActionTurnActorModel
    {
        public Card.Card Card;
        public int FactionIndex;
        public int ActorIndex;
        public int AttackSpeed;
        public int ActionPoint;

        public bool IsAvailable()
        {
            return ActionPoint >= 1000 && !Card.Battle.IsDead;
        }

        public void ResetAP() => ActionPoint = 0;

        public void AccumulateAP()
        {
            if (AttackSpeed <= 0)
            {
                throw new Exception("Speed must be greater than 0");
            }

            ActionPoint += AttackSpeed;
        }
    }

    [System.Serializable]
    public class ActionTurnManager
    {
        public List<ActionTurnActorModel> OriginalOrders = new List<ActionTurnActorModel>();
        public List<ActionTurnActorModel> ActionAvailableOrders = new List<ActionTurnActorModel>();
        public int MaxRoundCount;
        public int CurrentRoundCount;
        public ActionTurnActorModel CurrentTurn;

        public ActionTurnManager()
        {
            CurrentRoundCount = 0;
        }

        public void SetupOrders(List<BoardFaction> factions)
        {
            OriginalOrders = new List<ActionTurnActorModel>();
            int factionIndex = 1; // Bắt đầu từ 1

            foreach (var faction in factions)
            {
                foreach (var pos in faction.GetPositions())
                {
                    var actorIndex = pos.Index;
                    var cardModel = pos.Card.CardModel;

                    OriginalOrders.Add(new ActionTurnActorModel()
                    {
                        Card = pos.Card,
                        FactionIndex = factionIndex, // factionIndex đúng giá trị
                        ActorIndex = actorIndex,
                        AttackSpeed = cardModel.CalculatedAttributes
                            .Find(i => i.Type == AttributeType.AttackSpeed)?.Value ?? 0
                    });
                }

                factionIndex++; // Tăng factionIndex sau mỗi faction, HIỆN chỉ có 2
            }

            UpdateOrderIndex();
        }

        public void UpdateOrderIndex()
        {
            if (OriginalOrders.Count == 0) return;
            if (PreviousIndex() != ActionAvailableOrders.Count - 1) return;
            //dùng while để đảm bảo có ít nhất một người được hành đồng trong round
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
            }

            ActionAvailableOrders = OriginalOrders
                .Where(i => i.IsAvailable())
                .OrderByDescending(i => i.ActionPoint)
                .ToList();
            CurrentRoundCount++;
            Debug.Log($"Round {CurrentRoundCount}\n" +
                      $"Available: {ActionAvailableOrders.Count}");
        }

        public int PreviousIndex() => (CurrentTurn == null) ? 0 : ActionAvailableOrders.IndexOf(CurrentTurn);

        public ActionTurnActorModel GetNextTurn()
        {
            if (ActionAvailableOrders.Count == 0) return null;
            int previousIndex = PreviousIndex();

            // Duyệt qua danh sách để tìm nhân vật có thể hành động
            for (int i = 1; i <= ActionAvailableOrders.Count; i++)
            {
                int nextIndex = (previousIndex + i) % ActionAvailableOrders.Count;
                CurrentTurn = ActionAvailableOrders[nextIndex];
                if (!CurrentTurn.IsAvailable()) continue;
                return CurrentTurn;
            }

            Debug.LogError($"Turn bị null\n" +
                           $"ActionAvailableOrders: {ActionAvailableOrders.Count} (chết: {ActionAvailableOrders.Count(i=>!i.Card.Battle.IsDead)}");
            return null; // Không có ai có thể hành động
        }

        public bool IsRoundOver()
        {
            return CurrentRoundCount >= MaxRoundCount;
        }
    }
}