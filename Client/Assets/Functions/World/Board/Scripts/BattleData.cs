using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using World.TheCard;

namespace World.Board
{
    public class BattleData : MonoBehaviour, IGlobal
    {
        [SerializeField] public Board Board;
        [SerializeField] public ActionTurnManager ActionTurnManager;
        public async UniTask Init()
        {
            
        }
    }
}