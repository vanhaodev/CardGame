using System.Collections.Generic;
using UnityEngine.Serialization;

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
    }
}