using UnityEngine.Serialization;

namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillEffectActiveTriggerModel
    {
        /// <summary>
        /// Cái effect này có được kích hoạt hay không thì sẽ dựa vào cách caster thi triển lên địch hay bản thân hay team
        /// </summary>
        public SkillTargetType TargetType;
    }
}