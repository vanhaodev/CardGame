using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Globals;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.TheCard;
using World.Requirement;

namespace GameConfigs
{
    public partial class GameConfig : MonoBehaviour, IGlobal
    {
        public async UniTask Init()
        {
            var tasks = new List<UniTask>();
            tasks.Add(InitCard());
            tasks.Add(InitItem());
            await UniTask.WhenAll(tasks);
        }
    }
}