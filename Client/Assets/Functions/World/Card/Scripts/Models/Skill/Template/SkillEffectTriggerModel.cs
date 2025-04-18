namespace World.Card.Skill
{
    [System.Serializable]
    public class SkillEffectTriggerModel
    {
        /// <summary>
        /// Cái effect này có được kích hoạt hay không thì sẽ dựa vào cách caster thi triển lên địch hay bản thân hay team
        /// </summary>
        public SkillEffectTriggerType TriggerType;
        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToLeftCount là 2 thì card số 2 và 3 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        public byte AOEToLeftCount;
        /// <summary>
        /// ví dụ main target đang đứng ở card số 4 từ trái sang, nếu AOEToRightCount là 1 thì card số 5 từ trái sang sẽ ảnh hưởng theo
        /// </summary>
        public byte AOEToRightCount;
    }
}