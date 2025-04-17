using System.Collections.Generic;

using World.Card.Skill;

namespace World.Card
{
    // public class ParameterVisitor : LogicalExpressionVisitor
    // {
    //     public HashSet<string> UsedParameters { get; } = new();
    //
    //     public override void Visit(Identifier identifier)
    //     {
    //         UsedParameters.Add(identifier.Name);
    //     }
    // }
    // public class BattleFormular
    // {
    //     public static HashSet<string> GetUsedParameters(string formula)
    //     {
    //         var expression = new Expression(formula);
    //         var visitor = new ParameterVisitor();
    //     
    //         // Duyệt cây biểu thức để lấy biến
    //         expression.ParsedExpression.Accept(visitor);
    //     
    //         return visitor.UsedParameters;
    //     }
    //     public int Damage(Card attacker, List<SkillDamageTemplateModel> effects)
    //     {
    //         int totalDamage = 0;
    //         
    //         return totalDamage;
    //     }
    // }
    //
}