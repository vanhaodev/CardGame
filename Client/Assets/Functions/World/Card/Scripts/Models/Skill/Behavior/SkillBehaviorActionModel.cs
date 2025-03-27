using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.TextCore.Text;

namespace World.Card
{
    public abstract class SkillBehaviorActionModel
    {
        public bool Await { get; set; }
        public bool ProcessTargetsSequentially { get; set; }

        public abstract UniTask Execute(Card actor, List<Card> targets);
    }

}