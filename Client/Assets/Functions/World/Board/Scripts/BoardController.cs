using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using FloatingEffect;
using Globals;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using Utils;
using Random = UnityEngine.Random;

namespace World.Board
{
    public partial class BoardController : MonoBehaviour
    {
        [SerializeField] CinemachineCamera _camera;
        [SerializeField] private Transform _transFormCameraCenterPoint;
        [SerializeField] Board _board;

        /// <summary>
        /// the actor slibing index need to highest in canvas
        /// </summary>
        [SerializeField] private Transform _cardActingContainer;

        /// <summary>
        /// the actor slibing index need to highest in canvas but under actor
        /// </summary>
        [SerializeField] private Transform _cardTargetContainer;

        [SerializeField] CinemachineCameraShake _cameraShake;
        private Tween _tweenAction;
        private CancellationTokenSource _ctsTestLoop;

        private void Start()
        {
            _txRound.text = String.Empty;
            _txTurnTimeCountdown.text = String.Empty;
        }

        public void OnGUI()
        {
            // Tạo một button tại vị trí (100, 100) với kích thước 200x50
            if (GUI.Button(new Rect(100, 100, 200, 200), "Test!"))
            {
                TestLoop();
                Event.current.Use(); // Chặn sự kiện chuột
            }
        }

        [Button]
        public async void TestLoop()
        {
            _ctsTestLoop?.Cancel();
            _ctsTestLoop = new CancellationTokenSource();

            for (int i = 0; i < 6; i++)
            {
                var pos = _board.GetFaction(1).GetPosition(i + 1);
                pos.Card.Battle.SetupBattle(pos.Card, 1, i + 1);
            }

            for (int i = 0; i < 6; i++)
            {
                var pos = _board.GetFaction(2).GetPosition(i + 1);
                pos.Card.Battle.SetupBattle(pos.Card, 2, i + 1);
            }

            _actionTurn.SetupOrders(_board.GetFactions());
            _actionTurn.MaxRoundCount = 99;
            while (!_ctsTestLoop.IsCancellationRequested)
            {
                var result = await PlayAction();
                if (result != null)
                {
                    Debug.Log(JsonConvert.SerializeObject(result));
                    break;
                }
            }

            PerformCameraReset().Forget();
        }


        private List<int> GetTargets(BoardFaction targetFaction)
        {
            List<int> validIndexes = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                var theTarget = targetFaction.GetPosition(i + 1);
                if (!theTarget.Card.Battle.IsDead)
                {
                    validIndexes.Add(i + 1);
                }
            }

            int targetCount = 1;//Random.Range(1, 3); // allow skill target count
            Shuffle(validIndexes); // Xáo trộn danh sách

            return validIndexes.Take(Mathf.Min(targetCount, validIndexes.Count)).ToList();
        }

        // Hàm xáo trộn danh sách (Fisher-Yates Shuffle)
        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private float GetVerticalRotationAngle(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized;
            return -Mathf.Atan2(direction.z, direction.y) * Mathf.Rad2Deg;
        }


        public async void Freeze(float duration, float slowFactor = 0.05f)
        {
            Time.timeScale = slowFactor; // Làm chậm toàn bộ game
            await Task.Delay((int)(duration * 1000)); // Chờ theo thời gian thực
            Time.timeScale = 1f; // Khôi phục tốc độ bình thường
        }
    }
}