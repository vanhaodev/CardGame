using Cysharp.Threading.Tasks;

namespace World.Card
{
    public class SkillBehaviorRotateForwardModel : SkillBehaviorMoveForwardModel
    {
        /// <summary>
        /// Nếu true, luôn giữ đầu hướng lên trên (tránh bị lộn ngược)
        /// </summary>
        public bool IsAlwaysKeepUpright;
        public override async UniTask Execute(Card actor, Card target)
        {
            float offsetY = GetOffsetY(actor.transform.position, target.transform.position);
            
        }
    }
}