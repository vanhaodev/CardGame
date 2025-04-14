using System;
using UnityEngine.Serialization;

namespace World.Board
{
    [System.Serializable]
    public class ActionTurnActorModel
    {
        /// <summary>
        /// Mức AP cần để được hành động trong round
        /// </summary>
        private const int AP_REQUIRED_TO_ACTION = 1000;
        /// <summary>
        /// battle ref
        /// </summary>
        public Card.Card Card;
        public int FactionIndex;
        /// <summary>
        /// xem trong BoardFactionPosition
        /// </summary>
        public int MemberIndex;
        public int AttackSpeed;
        /// <summary>
        /// thứ tự trong round xếp theo AP cao đến thấp và phải lớn hơn AP_NEED
        /// </summary>
        public int ActionPoint;

        public bool IsAvailable()
        {
            return ActionPoint >= AP_REQUIRED_TO_ACTION && !Card.Battle.IsDead;
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