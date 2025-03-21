using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BoardController : MonoBehaviour
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

        void OnGUI()
        {
            // Tạo một button tại vị trí (100, 100) với kích thước 200x50
            if (GUI.Button(new Rect(100, 100, 200, 200), "Test!"))
            {
                TestLoop();
            }
        }

        [Button]
        public async void TestLoop()
        {
            for (int i = 0; i < 9; i++)
            {
                var battler = _board.GetFaction(1).GetPosition(i + 1);
                battler.Card.Battle.SetupBattle(battler.Card);
            }

            for (int i = 0; i < 9; i++)
            {
                var battler = _board.GetFaction(2).GetPosition(i + 1);
                battler.Card.Battle.SetupBattle(battler.Card);
            }

            while (!destroyCancellationToken.IsCancellationRequested)
            {
                var result = await PlayAction();
                if (result != null)
                {
                    Debug.Log(JsonConvert.SerializeObject(result));
                    break;
                }

                await Task.Delay(200);
            }
        }

        private Tween tween;

        private List<int> GetTargets(BoardFaction targetFaction)
        {
            List<int> validIndexes = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                var theTarget = targetFaction.GetPosition(i + 1);
                if (!theTarget.Card.Battle.IsDead)
                {
                    validIndexes.Add(i + 1);
                }
            }

            int targetCount = Random.Range(1, 3); // allow skill target count
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

        private int GetActorIndex(BoardFaction actorFaction)
        {
            List<int> validIndexes = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                var theTarget = actorFaction.GetPosition(i + 1);
                if (!theTarget.Card.Battle.IsDead)
                {
                    validIndexes.Add(i + 1);
                }
            }

            return validIndexes.Count > 0 ? validIndexes[UnityEngine.Random.Range(0, validIndexes.Count)] : 0;
        }


        public async Task<BoardEndResultModel> PlayAction()
        {
            var testAction = new CardActionModel
            {
                ActorFaction = Random.Range(1, 3),
            };
            //actor
            int hitCount = 1;
            var actorFaction = _board.GetFaction(testAction.ActorFaction);
            var actorIndex = GetActorIndex(actorFaction);
            if (actorIndex == 0)
            {
                return new BoardEndResultModel()
                {
                    IsEnd = true, WinFactionIndex = testAction.ActorFaction == 1 ? 2 : 1
                };
            }

            testAction.ActorIndex = GetActorIndex(actorFaction);
            var actor = actorFaction.GetPosition(testAction.ActorIndex);
            //set cam
            _camera.Follow = actor.Card.transform;
            await DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, 4.2f, 0.5f)
                .SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            //=================================================================================

            //target
            var targetFaction = _board.GetFaction(testAction.ActorFaction == 1 ? 2 : 1);
            testAction.TargetIndex = GetTargets(targetFaction);
            var originalActorContainer = actor.Card.transform.parent;
            //=================================================================================

            Vector3 originalPosition = actor.Card.transform.position; // Lưu vị trí gốc

            // Chuyển card vào container hành động mà không thay đổi scale/rotation
            actor.Card.transform.SetParent(_cardActingContainer, false);

            actor.Card.Battle.Vital.Show(false);
            foreach (var target in testAction.TargetIndex)
            {
                var targetCard = targetFaction.GetPosition(target).Card.transform;
                Vector3 targetPosition = targetCard.position;
                Vector3 actorPosition = actor.Card.transform.position;
                var originalTargetContainer = targetCard.parent;
                targetCard.transform.SetParent(_cardTargetContainer, false);
                // Nếu actor ở trên target thì +1 Y, nếu ở dưới thì -1 Y
                float offsetY = actorPosition.y > targetPosition.y ? 1.0f : -1.0f;
                Vector3 attackPosition = new Vector3(targetPosition.x, targetPosition.y + offsetY, targetPosition.z);

                // Di chuyển đến vị trí gần mục tiêu
                await actor.Card.transform.DOMove(attackPosition, 0.5f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
                for (int i = 0; i < hitCount; i++)
                {
                    bool isActingDone = false;
                    bool isShowFloatingEffect = false;
                    // Xoay thẻ bài như động tác chém (giả sử xoay 70 độ)
                    tween = actor.Card.transform.DORotate(new Vector3(0, 0, offsetY < 0 ? 80 : -80), 0.2f)
                        .SetEase(Ease.InQuad).OnPlay(
                            () => { Global.Instance.Get<SoundManager>().PlaySoundOneShot(2); }).OnUpdate(async () =>
                        {
                            if (tween.ElapsedPercentage() >= 0.5f)
                            {
                                if (isShowFloatingEffect == false)
                                {
                                    int damage = Random.Range(0, 100);
                                    targetFaction.GetPosition(target).Card.Battle.OnTakeDamage(damage);
                                    Global.Instance.Get<FloatingEffectManager>().ShowDamage(damage,
                                        targetPosition);
                                    Global.Instance.Get<FloatingEffectManager>().ShowSlash(
                                        targetPosition);
                                    isShowFloatingEffect = true;
                                }

                                // Debug.Log(tween.ElapsedPercentage());
                                //target ngã ra sau
                                Vector3 takeHitPos = new Vector3(targetPosition.x, targetPosition.y - (offsetY / 2),
                                    targetPosition.z);
                                await targetCard.transform.DOMove(takeHitPos, 0.3f).OnPlay(() =>
                                {
                                    _cameraShake.Shake(2f, 3f, 0.2f); // Attack shake
                                    // Freeze(0.2f);
                                }).AsyncWaitForCompletion();

                                isActingDone = true;
                            }
                        });
                    await tween.AsyncWaitForCompletion();
                    await UniTask.WaitUntil(() => isActingDone);
                    // Xoay lại vị trí ban đầu
                    await actor.Card.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad)
                        .AsyncWaitForCompletion();
                }

                await targetCard.transform.DOMove(targetPosition, 0.3f)
                    .AsyncWaitForCompletion();
                targetCard.transform.SetParent(originalTargetContainer, false);
            }

            // Quay về vị trí gốc
            await actor.Card.transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InQuad).AsyncWaitForCompletion();

            // Trả card về container cũ
            actor.Card.transform.SetParent(originalActorContainer, false);
            actor.Card.Battle.Vital.Show();
            await DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, 7.2f, 0.5f)
                .OnPlay(() => _camera.Follow = _transFormCameraCenterPoint)
                .SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            return null;
        }

        public async void Freeze(float duration, float slowFactor = 0.05f)
        {
            Time.timeScale = slowFactor; // Làm chậm toàn bộ game
            await Task.Delay((int)(duration * 1000)); // Chờ theo thời gian thực
            Time.timeScale = 1f; // Khôi phục tốc độ bình thường
        }
    }
}