using System.Collections.Generic;
using UnityEngine;

namespace World.TheCard.Skill
{
    public class SkillAttributeTriggerModel : SkillEffectTriggerModel
    {
        public enum SkillAttributeTriggerCompareType
        {
            LessThan,
            EqualTo,
            GreaterThan,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo,
        }

        public SkillAttributeTriggerCompareType CompareType;
        public List<AttributeModel> Attributes;
        public float HpPercent = -1;

        public override bool IsSatisfied(Card sender, Card receiver)
        {
            var checks = GetCheckableCards(sender, receiver);

            foreach (var card in checks)
            {
                var attrs = card.Battle.Attributes;

                if (HpPercent > -1)
                {
                    float percent = (float)attrs[AttributeType.Hp] / attrs[AttributeType.HpMax];
                    if (percent <= HpPercent)
                        return true;
                }

                foreach (var att in Attributes)
                {
                    if (attrs.TryGetValue(att.Type, out var value))
                    {
                        if (CompareAttributes(value, att.Value))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool CompareAttributes(int actual, int expected)
        {
            switch (CompareType)
            {
                case SkillAttributeTriggerCompareType.LessThan:
                    return actual < expected;
                case SkillAttributeTriggerCompareType.EqualTo:
                    return actual == expected;
                case SkillAttributeTriggerCompareType.GreaterThan:
                    return actual > expected;
                case SkillAttributeTriggerCompareType.GreaterThanOrEqualTo:
                    return actual >= expected;
                case SkillAttributeTriggerCompareType.LessThanOrEqualTo:
                    return actual <= expected;
                default:
                    return false;
            }
        }
    }
}