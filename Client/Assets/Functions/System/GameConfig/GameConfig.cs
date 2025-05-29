using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;

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

        public (string mainColor, string gradient, string gradient2) GetStarColor(byte star)
        {
            return star switch
            {
                0 => ("#F5F5F5", "#F5F5F5", "#CCCCCC"),       // Trắng
                1 => ("#F5F5F5", "#F5F5F5", "#CCCCCC"),       // Trắng
                2 => ("#66CCFF", "#66CCFF", "#0066CC"),       // Xanh
                3 => ("#C580FF", "#C580FF", "#6B1EFF"),       // Tím
                4 => ("#FFE066", "#FFE066", "#FF9900"),       // Vàng
                5 => ("#FF9966", "#FF9966", "#FF3300"),       // Cam
                _ => ("#F5F5F5", "#F5F5F5", "#CCCCCC"),       // Trắng
            };
        }
    }
}