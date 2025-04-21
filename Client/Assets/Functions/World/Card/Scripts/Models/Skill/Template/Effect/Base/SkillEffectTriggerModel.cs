using System.Collections.Generic;
using UnityEngine.Serialization;

namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillEffectTriggerModel
    {
        public List<TriggerModel> Triggers;

        [System.Serializable]
        public class TriggerModel
        {
            public TriggerType Type;
            public int IntValue;
            public string StringValue;
        }
        public enum TriggerType
        {
            TargetIsTeam,
            TargetIsSelf,
            TargetIsTeamAndSelf,
            TargetIsEnemy,
            /// <summary>
            /// HP mục tiêu nhỏ hơn x
            /// </summary>
            HPLessThan,
            /// <summary>
            /// HP mục tiêu lớn hơn x
            /// </summary>
            HPGreaterThan,
            /// <summary>
            /// HP đang có % nhỏ hơn x
            /// </summary>
            HPLessThanPercent,
            /// <summary>
            /// HP đang có % lớn hơn x
            /// </summary>
            HPGreaterThanPercent,
            /// <summary>
            /// UP nhỏ hơn x
            /// </summary>
            UPLessThan,
            /// <summary>
            /// UP lớn hơn x
            /// </summary>
            UPGreaterThan,
            
        }
    }
}