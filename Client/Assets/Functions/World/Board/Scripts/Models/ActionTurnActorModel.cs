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

        public bool IsAvailable()
        {
            return Card.Battle.ActionPoint >= AP_REQUIRED_TO_ACTION && !Card.Battle.IsDead;
        }

        public void ResetAP() => Card.Battle.ResetAP();

        public void AccumulateAP() => Card.Battle.AccumulateAP();
    }
}