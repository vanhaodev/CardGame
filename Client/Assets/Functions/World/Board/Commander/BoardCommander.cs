using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;

namespace World.Board
{
    public class BoardCommander : MonoBehaviour, IGlobal
    {
        private bool _isSelectingTarget;
        public bool IsSelectingTarget() => _isSelectingTarget;

        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }
}