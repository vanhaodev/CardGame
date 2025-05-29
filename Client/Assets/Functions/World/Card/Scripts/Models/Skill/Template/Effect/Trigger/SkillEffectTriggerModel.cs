using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using World.Board;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public abstract class SkillEffectTriggerModel
    {
        public enum SkillEffectTriggerCheckType
        {
            /// <summary>
            /// Kiểm tra người dính chiêu
            /// </summary>
            CheckTarget,

            /// <summary>
            /// Kiểm tra người thi triển
            /// </summary>
            CheckSender,

            /// <summary>
            /// Kiểm tra team của người thi triển
            /// </summary>
            CheckSenderTeam,

            /// <summary>
            /// Check người thi triển lẫn team của hắn
            /// </summary>
            CheckSenderAndSenderTeam,

            /// <summary>
            /// Kiểm trả team của người dính chiêu
            /// </summary>
            CheckTargetTeam,

            /// <summary>
            /// Check cả người dính chiêu lẫn team của hắn
            /// </summary>
            CheckTargetAndTargetTeam,

            /// <summary>
            /// Kiểm tra all người cả 2 phe
            /// </summary>
            CheckAllBattlers
        }

        public SkillEffectTriggerCheckType CheckType;
        public abstract bool IsSatisfied(CardBattle sender, CardBattle receiver);

        public List<CardBattle> GetCheckableCards(CardBattle sender, CardBattle receiver)
        {
            var board = Globals.Global.Instance.Get<BattleData>().Board;
            switch (CheckType)
            {
                case SkillEffectTriggerCheckType.CheckTarget:
                {
                    return new List<CardBattle> { receiver };
                }
                case SkillEffectTriggerCheckType.CheckSender:
                {
                    return new List<CardBattle> { sender };
                }
                case SkillEffectTriggerCheckType.CheckSenderTeam:
                {
                    return board
                        .GetFactionByIndex(sender.FactionIndex)
                        .GetAllPositions()
                        .Where(i => i.CardBattle != sender)
                        .Select(i => i.CardBattle)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckSenderAndSenderTeam:
                {
                    return board
                        .GetFactionByIndex(sender.FactionIndex)
                        .GetAllPositions()
                        .Select(i => i.CardBattle)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckTargetTeam:
                {
                    return board
                        .GetFactionByIndex(receiver.FactionIndex)
                        .GetAllPositions()
                        .Where(i => i.CardBattle != receiver)
                        .Select(i => i.CardBattle)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckTargetAndTargetTeam:
                {
                    return board
                        .GetFactionByIndex(receiver.FactionIndex)
                        .GetAllPositions()
                        .Select(i => i.CardBattle)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckAllBattlers:
                {
                    var factions = board.GetAllFactions();
                    var allPositions = factions.SelectMany(f => f.GetAllPositions());
                    var allCards = allPositions
                        .Select(p => p.CardBattle)
                        .Where(c => c != null)
                        .ToList();
                    return allCards;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}