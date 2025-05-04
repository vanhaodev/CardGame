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
        public abstract bool IsSatisfied(Card sender, Card receiver);

        public List<Card> GetCheckableCards(Card sender, Card receiver)
        {
            var board = Globals.Global.Instance.Get<BattleData>().Board;
            switch (CheckType)
            {
                case SkillEffectTriggerCheckType.CheckTarget:
                {
                    return new List<Card> { receiver };
                }
                case SkillEffectTriggerCheckType.CheckSender:
                {
                    return new List<Card> { sender };
                }
                case SkillEffectTriggerCheckType.CheckSenderTeam:
                {
                    return board
                        .GetFactionByIndex(sender.Battle.FactionIndex)
                        .GetAllPositions()
                        .Where(i => i.Card != sender)
                        .Select(i => i.Card)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckSenderAndSenderTeam:
                {
                    return board
                        .GetFactionByIndex(sender.Battle.FactionIndex)
                        .GetAllPositions()
                        .Select(i => i.Card)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckTargetTeam:
                {
                    return board
                        .GetFactionByIndex(receiver.Battle.FactionIndex)
                        .GetAllPositions()
                        .Where(i => i.Card != receiver)
                        .Select(i => i.Card)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckTargetAndTargetTeam:
                {
                    return board
                        .GetFactionByIndex(receiver.Battle.FactionIndex)
                        .GetAllPositions()
                        .Select(i => i.Card)
                        .ToList();
                }
                case SkillEffectTriggerCheckType.CheckAllBattlers:
                {
                    var factions = board.GetAllFactions();
                    var allPositions = factions.SelectMany(f => f.GetAllPositions());
                    var allCards = allPositions
                        .Select(p => p.Card)
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