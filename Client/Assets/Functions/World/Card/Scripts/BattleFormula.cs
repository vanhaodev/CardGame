using System;
using System.Collections.Generic;
using System.Linq;
using NCalc;
using UnityEngine;
using World.TheCard.Skill;

namespace World.TheCard
{
    public class BattleFormula
    {
        /// <summary>
        /// Trích xuất danh sách tên tham số từ biểu thức.
        /// </summary>
        public static List<string> GetParamNames(string formula)
        {
            var expr = new Expression(formula);
            var foundParams = new HashSet<string>();

            expr.EvaluateParameter += (name, args) =>
            {
                foundParams.Add(name);
                args.Result = 1; // tránh lỗi khi evaluate
            };

            expr.Evaluate(); // cần thiết để trigger EvaluateParameter
            return foundParams.ToList();
        }

        /// <summary>
        /// Gán giá trị cho các tham số, bao gồm cả Level và xử lý các chỉ số có tỉ lệ (Chance).
        /// </summary>
        public void SetParamValue(Dictionary<string, int> parameters, ushort senderLevel, ushort receiverLevel,
            Dictionary<AttributeType, int> senderAttributes, Dictionary<AttributeType, int> receiverAttributes)
        {
            // Các cặp tham số có Chance
            var chancePairs = new Dictionary<string, string>
            {
                { AttributeType.CriticalDamage.ToString(), AttributeType.CriticalRate.ToString() },
                { AttributeType.DodgeDamage.ToString(), AttributeType.DodgeRate.ToString() },
                { AttributeType.ArmorPenetrationDamage.ToString(), AttributeType.ArmorPenetrationChance.ToString() }
            };

            foreach (var key in parameters.Keys.ToList())
            {
                // Nếu là S_Level hoặc R_Level thì xử lý riêng
                if (key == "S_Level")
                {
                    parameters[key] = senderLevel;
                    continue;
                }
                else if (key == "R_Level")
                {
                    parameters[key] = receiverLevel;
                    continue;
                }

                // Xử lý các thuộc tính khác
                string rawName = key.StartsWith("S_") || key.StartsWith("R_") ? key.Substring(2) : key;
                var prefix = key.Substring(0, 2); // "S_" hoặc "R_"

                Dictionary<AttributeType, int> sourceAttributes = null;
                if (prefix == "S_") sourceAttributes = senderAttributes;
                else if (prefix == "R_") sourceAttributes = receiverAttributes;

                if (sourceAttributes != null && Enum.TryParse(rawName, true, out AttributeType attrType))
                {
                    int value = sourceAttributes.TryGetValue(attrType, out var val) ? val : 0;

                    // Nếu là một loại Damage có đi kèm Chance
                    if (chancePairs.TryGetValue(rawName, out var chanceName))
                    {
                        string chanceKey = prefix + chanceName;

                        if (parameters.ContainsKey(chanceKey))
                        {
                            if (Enum.TryParse(chanceName, true, out AttributeType chanceAttr))
                            {
                                int chanceValue = sourceAttributes.TryGetValue(chanceAttr, out var cval) ? cval : 0;
                                bool passed = UnityEngine.Random.Range(0, 10000) < chanceValue;

                                value = passed ? value : 1; // Trượt tỉ lệ thì chỉ gây 1 damage
                            }
                        }
                    }

                    parameters[key] = value;
                }
            }
        }

        /// <summary>
        /// Tính tổng sát thương từ attacker gây lên victim, dựa trên danh sách effect công thức.
        /// </summary>
        public int CalDamage(Card sender, Card receiver, List<SkillDamageTemplateModel> effects)
        {
            int totalDamage = 0;

            foreach (var effect in effects)
            {
                var paramNames = GetParamNames(effect.Formula);
                var paramValues = new Dictionary<string, int>();

                // Khởi tạo tham số rỗng
                foreach (var name in paramNames)
                    paramValues[name] = 0;

                // Gán giá trị tương ứng
                SetParamValue(paramValues, sender.CardModel.Level.GetLevel(), receiver.CardModel.Level.GetLevel(),
                    sender.Battle.Attributes, receiver.Battle.Attributes);

                // Tính toán kết quả
                var expr = new Expression(effect.Formula);
                foreach (var kv in paramValues)
                    expr.Parameters[kv.Key] = kv.Value;

                totalDamage += (int)expr.Evaluate();
            }

            return totalDamage;
        }

        public int CalModifyAttribute(Card sender, Card reciever, List<SkillModifyAttributeTemplateModel> effects)
        {
            int totalDamage = 0;

            foreach (var effect in effects)
            {
                var paramNames = GetParamNames(effect.Formula);
                var paramValues = new Dictionary<string, int>();

                // Khởi tạo tham số rỗng
                foreach (var name in paramNames)
                    paramValues[name] = 0;

                // Gán giá trị tương ứng
                SetParamValue(paramValues, sender.CardModel.Level.GetLevel(), reciever.CardModel.Level.GetLevel(),
                    sender.Battle.Attributes, reciever.Battle.Attributes);

                // Tính toán kết quả
                var expr = new Expression(effect.Formula);
                foreach (var kv in paramValues)
                    expr.Parameters[kv.Key] = kv.Value;

                totalDamage += (int)expr.Evaluate();
            }

            return totalDamage;
        }
    }
}