using System;

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
            if(Card.Battle.IsDead) return;
            if (AttackSpeed <= 0)
            {
                throw new Exception("Speed must be greater than 0");
            }

            ActionPoint += AttackSpeed;
        }
    }

}