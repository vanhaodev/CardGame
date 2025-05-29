using System;
using UnityEngine.Serialization;
using World.TheCard;

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
        public CardBattle CardBattle;

        public bool IsAvailable()
        {
            return CardBattle.BattleAttributes[BattleAttributeType.ActionPoint] >= AP_REQUIRED_TO_ACTION &&
                   !CardBattle.IsDead;
        }

        public void ResetAP() => CardBattle.ResetAP();

        public void AccumulateAP() => CardBattle.AccumulateAP();
    }
}