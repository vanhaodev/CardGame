using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using FloatingEffect;
using Globals;
using Sirenix.OdinInspector;
using Utils;

namespace World.Board
{
    public class BoardController : MonoBehaviour
    {
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

        [Button]
        public async void TestLoop()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                await PlayAction();
                await Task.Delay(200);
            }
        }

        private Tween tween;

        public async Task PlayAction()
        {
            var testAction = new CardActionModel
            {
                ActorFaction = Random.Range(1, 3),
                ActorIndex = Random.Range(1, 7),
                TargetIndex = new List<int>
                {
                    Random.Range(1, 7)
                }
            };
            int hitCount = 1;
            var actorFaction = _board.GetFaction(testAction.ActorFaction);
            var actor = actorFaction.GetPosition(testAction.ActorIndex);
            var targetFaction = _board.GetFaction(testAction.ActorFaction == 1 ? 2 : 1);
            var originalActorContainer = actor.Card.transform.parent;

            Vector3 originalPosition = actor.Card.transform.position; // Lưu vị trí gốc

            // Chuyển card vào container hành động mà không thay đổi scale/rotation
            actor.Card.transform.SetParent(_cardActingContainer, false);

            actor.Card.ShowVital(false);
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
                                    Global.Instance.Get<FloatingEffectManager>().ShowDamage(Random.Range(0, 1000000),
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
            actor.Card.ShowVital();
        }

        public async void Freeze(float duration, float slowFactor = 0.05f)
        {
            Time.timeScale = slowFactor; // Làm chậm toàn bộ game
            await Task.Delay((int)(duration * 1000)); // Chờ theo thời gian thực
            Time.timeScale = 1f; // Khôi phục tốc độ bình thường
        }
    }
}