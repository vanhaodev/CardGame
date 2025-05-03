using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Effects;
using Globals;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Cinemachine;
using Utils;
using Random = UnityEngine.Random;

namespace World.Board
{
    public partial class BoardController : MonoBehaviour
    {
        //------------ Comp ---------------\\
        [SerializeField] [BoxGroup("Main")] CinemachineCamera _camera;
        [SerializeField] [BoxGroup("Main")] private Transform _transFormCameraCenterPoint;
        [SerializeField] [BoxGroup("Main")] Board _board;
        [SerializeField] [BoxGroup("Main")] TargetSelectorController _targetSelectorController;
        [SerializeField] [BoxGroup("Main")] CinemachineCameraShake _cameraShake;

        //------------ Entity ---------------\\
        /// <summary>
        /// the actor slibing index need to highest in canvas
        /// </summary>
        [SerializeField] [BoxGroup("Main")] private Transform _cardActingContainer;

        /// <summary>
        /// the actor slibing index need to highest in canvas but under actor
        /// </summary>
        [SerializeField] [BoxGroup("Main")] private Transform _cardTargetContainer;

        //----------- Data -------------\\
        private Tween _tweenAction;
        private CancellationTokenSource _ctsTestLoop;

        private void Start()
        {
            Global.Instance.Get<BattleData>().ActionTurnManager = _actionTurnManager;
            _targetSelectorController.InitTargets(_board);
        }

        private void FixedUpdate()
        {
            if (_actionTurnManager != null)
            {
                var currentTurnCountDownTimeSecond = _actionTurnManager.UpdateTimeCountdown();
                _board.SetTurnCountDown(currentTurnCountDownTimeSecond);
            }
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
        public async UniTask SetupBoardCards()
        {
            var cardInitTasks = new List<UniTask>();
            for (int i = 0; i < 6; i++)
            {
                var pos = _board.GetFactionByIndex(1).GetPositionByIndex(i + 1);
                cardInitTasks.Add(pos.Card.Battle.SetupBattle(pos.Card, 1, i + 1));
            }

            for (int i = 0; i < 6; i++)
            {
                var pos = _board.GetFactionByIndex(2).GetPositionByIndex(i + 1);
                cardInitTasks.Add(pos.Card.Battle.SetupBattle(pos.Card, 2, i + 1));
            }

            await UniTask.WhenAll(cardInitTasks);
        }

        [Button]
        public async void TestLoop()
        {
            _ctsTestLoop?.Cancel();
            _ctsTestLoop = new CancellationTokenSource();

            await SetupBoardCards();

            _board.GetFactionByIndex(1).ResetToOriginalPosition();
            _board.GetFactionByIndex(2).ResetToOriginalPosition();
            _actionTurnManager.SetupOrders(_board.GetAllFactions());
            _actionTurnManager.MaxRoundCount = 99;
            int count = 0;
            while (!_ctsTestLoop.IsCancellationRequested)
            {
                var result = await PlayAction(_ctsTestLoop);
                if (result != null)
                {
                    Debug.Log(JsonConvert.SerializeObject(result));
                    break;
                }

                if (count >= 999) throw new Exception("loop is too large.");
                count++;
            }

            PerformCameraReset(_ctsTestLoop).Forget();
        }


        private List<int> GetTargets(BoardFaction targetFaction)
        {
            List<int> validIndexes = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                var theTarget = targetFaction.GetPositionByIndex(i + 1);
                if (!theTarget.Card.Battle.IsDead)
                {
                    validIndexes.Add(i + 1);
                }
            }

            int targetCount = 1; //Random.Range(1, 3); // allow skill target count
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


        public async void Freeze(float duration, CancellationTokenSource cts, float slowFactor = 0.05f)
        {
            Time.timeScale = slowFactor; // Làm chậm toàn bộ game
            await Task.Delay((int)(duration * 1000), cancellationToken: cts.Token); // Chờ theo thời gian thực
            Time.timeScale = 1f; // Khôi phục tốc độ bình thường
        }
    }
}