using System.Collections.Generic;
using System.Linq;
using World.Card;

namespace World.Board
{
    [System.Serializable]
    public class ActionTurnActorModel
    {
        public int FactionIndex;
        public int ActorIndex;
        public int AttackSpeed;

        public bool IsAvailable()
        {
            return true;
        }
    }

    [System.Serializable]
    public class ActionTurnModel
    {
        public List<ActionTurnActorModel> Orders = new List<ActionTurnActorModel>();
        public int MaxRoundCount;
        public int CurrentRoundCount;
        public ActionTurnActorModel CurrentTurn;

        public ActionTurnModel()
        {
            CurrentRoundCount = 1;
        }
        public void SetupOrders(List<BoardFaction> factions)
        {
            Orders = new List<ActionTurnActorModel>();
            int factionIndex = 1; // Bắt đầu từ 1

            foreach (var faction in factions)
            {
                foreach (var pos in faction.GetPositions())
                {
                    var actorIndex = pos.Index;
                    var cardModel = pos.Card.CardModel;

                    Orders.Add(new ActionTurnActorModel()
                    {
                        FactionIndex = factionIndex, // factionIndex đúng giá trị
                        ActorIndex = actorIndex,
                        AttackSpeed = cardModel.CalculatedAttributes
                            .Find(i => i.Type == AttributeType.AttackSpeed)?.Value ?? 0
                    });
                }

                factionIndex++; // Tăng factionIndex sau mỗi faction
            }

            Orders = Orders.OrderByDescending(i => i.AttackSpeed).ToList();
        }

        public ActionTurnActorModel GetNextTurn()
        {
            if (Orders.Count == 0) return null;
            int startIndex = (CurrentTurn == null) ? 0 : Orders.IndexOf(CurrentTurn);

            // Duyệt qua danh sách để tìm nhân vật có thể hành động
            for (int i = 1; i <= Orders.Count; i++)
            {
                int nextIndex = (startIndex + i) % Orders.Count;
                if (Orders[nextIndex].IsAvailable())
                {
                    CurrentTurn = Orders[nextIndex];
                    CurrentRoundCount++;
                    return CurrentTurn;
                }
            }

            return null; // Không có ai có thể hành động
        }

        public bool IsRoundOver()
        {
            return CurrentRoundCount >= MaxRoundCount;
        }
    }
}