using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FloatingEffect;
using Globals;
using UnityEngine;

namespace World.Board
{
    public partial class BoardController : MonoBehaviour
    {
        public async Task<BoardEndResultModel> PlayAction()
        {
            //========================[Setup Action]===============\
            var testAction = new CardActionModel
            {
                ActorFaction = Random.Range(1, 3),
            };

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


            //========================[Select Targets]===============\
            var targetFaction = _board.GetFaction(testAction.ActorFaction == 1 ? 2 : 1);
            testAction.TargetIndex = GetTargets(targetFaction);

            //========================[Perform Attacks]===============\
            if (Random.Range(0, 2) == 1)
            {
                await Ranged(actor,
                    testAction.TargetIndex.Select(target => targetFaction.GetPosition(target)).ToList());
            }
            else
            {
                await Melee(actor, testAction.TargetIndex.Select(target => targetFaction.GetPosition(target)).ToList());
            }

            //========================[Camera Reset]===============\
            await PerformCameraReset();
            return null;
        }

        public async UniTask Melee(BoardFactionPosition actor, List<BoardFactionPosition> targets)
        {
            //========================[Camera Focus]===============\
            await PerformCameraFocus(actor.Card.transform, 5.2f);
            //========================[Prepare Melee Attack]===============\
            Vector3 originalPosition = actor.Card.transform.position;

            //========================[Set parent]===============\
            var originalActorParent = actor.Card.transform.parent;
            actor.Card.transform.SetParent(_cardActingContainer, false);

            //========================[Hide vital bar]===============\
            actor.Card.Battle.Vital.Show(false);

            foreach (var target in targets)
            {
                //========================[Setup Attack]===============\
                var targetCard = target.Card.transform;
                Vector3 targetPosition = targetCard.position;
                Vector3 actorPosition = actor.Card.transform.position;

                //========================[Set parent]===============\
                var originalTargetParent = targetCard.parent;
                targetCard.transform.SetParent(_cardTargetContainer, false);

                //========================[Move to target]===============\
                float offsetY = actorPosition.y > targetPosition.y ? 1.0f : -1.0f;
                Vector3 attackPosition = new Vector3(targetPosition.x, targetPosition.y + offsetY, targetPosition.z);
                await actor.Card.transform.DOMove(attackPosition, 0.5f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

                bool isActingDone = false;
                bool isShowFloatingEffect = false;

                //========================[Perform Melee Attack Animation]===============\
                _tweenAction = actor.Card.transform.DORotate(new Vector3(0, 0, offsetY < 0 ? 80 : -80), 0.2f)
                    .SetEase(Ease.InQuad).OnPlay(() => Global.Instance.Get<SoundManager>().PlaySoundOneShot(2))
                    .OnUpdate(async () =>
                    {
                        if (_tweenAction.ElapsedPercentage() >= 0.5f && !isShowFloatingEffect)
                        {
                            int damage = Random.Range(0, 100);
                            target.Card.Battle.OnTakeDamage(damage);
                            Global.Instance.Get<FloatingEffectManager>().ShowDamage(damage, targetPosition);
                            Global.Instance.Get<FloatingEffectManager>().ShowSlash(targetPosition);
                            isShowFloatingEffect = true;

                            Vector3 takeHitPos = new Vector3(targetPosition.x, targetPosition.y - (offsetY / 2),
                                targetPosition.z);

                            //========================[Target act receive hit]===============\
                            await targetCard.transform.DOMove(takeHitPos, 0.3f)
                                .OnPlay(() => _cameraShake.Shake(2f, 3f, 0.2f))
                                .AsyncWaitForCompletion();

                            isActingDone = true;
                        }
                    });

                await _tweenAction.AsyncWaitForCompletion();
                await UniTask.WaitUntil(() => isActingDone);

                //========================[Reset Rotation]===============\
                await actor.Card.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
                //========================[Return to Position]===============\
                await targetCard.transform.DOMove(targetPosition, 0.3f).AsyncWaitForCompletion();

                //========================[Set parent]===============\
                targetCard.transform.SetParent(originalTargetParent, false);
            }

            //========================[Return to Position]===============\
            await actor.Card.transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InQuad).AsyncWaitForCompletion();
            //========================[Set parent]===============\
            actor.Card.transform.SetParent(originalActorParent, false);
            //========================[Show vital bar]===============\
            actor.Card.Battle.Vital.Show();
        }

        public async UniTask Ranged(BoardFactionPosition actor, List<BoardFactionPosition> targets)
        {
            //========================[Prepare Melee Attack]===============\
            Vector3 originalPosition = actor.Card.transform.position;

            //========================[Set parent]===============\
            var originalActorParent = actor.Card.transform.parent;
            actor.Card.transform.SetParent(_cardActingContainer, false);

            //========================[Hide vital bar]===============\
            actor.Card.Battle.Vital.Show(false);

            foreach (var target in targets)
            {
                //========================[Setup Attack]===============\
                var targetCard = target.Card.transform;
                Vector3 targetPosition = targetCard.position;
                Vector3 actorPosition = actor.Card.transform.position;

                //========================[Set parent]===============\
                var originalTargetParent = targetCard.parent;
                targetCard.transform.SetParent(_cardTargetContainer, false);

                //========================[Move to target]===============\
                float offsetY = actorPosition.y > targetPosition.y ? 1.0f : -1.0f;
                // Vector3 attackPosition = new Vector3(targetPosition.x, targetPosition.y + offsetY, targetPosition.z);
                // await actor.Card.transform.DOMove(attackPosition, 0.5f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

                bool isActingDone = false;

                bool isShowFloatingEffect = false;

                //========================[Perform Melee Attack Animation]===============\

                //=======================[Rotate]============================\
                Vector3 direction = targetCard.transform.position - actor.Card.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Thêm 90 độ để card quay theo top/bottom thay vì left/right
                if (offsetY > 0)
                {
                    angle += 90f;
                }
                else
                {
                    angle -= 90f;
                }

                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                await PerformCameraFocus(actor.Card.transform, 5.2f);
                await actor.Card.transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion();
                //

                _tweenAction = actor.Card.transform.DOMoveY(originalPosition.y + (offsetY / 3), 0.1f)
                    .SetEase(Ease.InQuad).OnPlay(() => Global.Instance.Get<SoundManager>().PlaySoundOneShot(2))
                    .OnUpdate(async () =>
                    {
                        if (_tweenAction.ElapsedPercentage() >= 0.5f && !isShowFloatingEffect)
                        {
                            isShowFloatingEffect = true;
                            int damage = Random.Range(0, 100);
                            PerformCameraReset().Forget();
                            await Global.Instance.Get<FloatingEffectManager>()
                                .ShowBullet(actorPosition, targetPosition);
                            target.Card.Battle.OnTakeDamage(damage);
                            Global.Instance.Get<FloatingEffectManager>().ShowDamage(damage, targetPosition);
                            Global.Instance.Get<FloatingEffectManager>().ShowSlash(targetPosition);

                            Vector3 takeHitPos = new Vector3(targetPosition.x, targetPosition.y - (offsetY / 2),
                                targetPosition.z);

                            //========================[Target act receive hit]===============\
                            await targetCard.transform.DOMove(takeHitPos, 0.3f)
                                .OnPlay(() => _cameraShake.Shake(2f, 3f, 0.2f))
                                .AsyncWaitForCompletion();

                            isActingDone = true;
                        }
                    });

                await _tweenAction.AsyncWaitForCompletion();
                await UniTask.WaitUntil(() => isActingDone);

                //========================[Reset Move]===============\
                await actor.Card.transform.DOMoveY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion();
                await actor.Card.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

                //========================[Return to Position]===============\
                await targetCard.transform.DOMove(targetPosition, 0.3f).AsyncWaitForCompletion();

                //========================[Set parent]===============\
                targetCard.transform.SetParent(originalTargetParent, false);
            }

            //========================[Return to Position]===============\
            await actor.Card.transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InQuad).AsyncWaitForCompletion();
            //========================[Set parent]===============\
            actor.Card.transform.SetParent(originalActorParent, false);
            //========================[Show vital bar]===============\
            actor.Card.Battle.Vital.Show();
        }

        private async UniTask PerformCameraFocus(Transform target, float zoom)
        {
            // _camera.Follow = target;
            // _cinemachineConfiner.InvalidateBoundingShapeCache();
            await DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, zoom, 0.5f)
                .OnPlay(() => { _camera.Follow = target; })
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();
        }

        private async UniTask PerformCameraReset()
        {
            await DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, 7.2f, 0.5f)
                .OnPlay(() => { _camera.Follow = _transFormCameraCenterPoint; })
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();
        }
    }
}