using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Effects;
using Globals;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using World.TheCard;
using Random = UnityEngine.Random;

namespace World.Board
{
    public partial class BoardController : MonoBehaviour
    {
        //------------------ Overview ------------\\
        //Flow: Start -> Xếp timeline order -> Chờ battler ra lệnh -> thực hiện lệnh -> d
        //------------------ Display --------------\\

        //------------------ Comp ------------------\\
        [SerializeField] [BoxGroup("Action")] ActionTurnManager _actionTurnManager;

        public async Task<RoundResultModel> PlayAction(CancellationTokenSource cts)
        {
            //========================[1. Kiểm tra điều kiện bắt đầu round]========================
            if (!SetupRound())
            {
                return new RoundResultModel { WinFactionIndex = 0, Debug = "Round over" }; // Trận hòa
            }

            //========================[2. Kiểm tra phe thắng/thua]========================
            var roundResult = GetRoundResult();
            if (roundResult.WinFactionIndex != 0)
            {
                return roundResult; // Trả kết quả nếu đã có phe thắng
            }

            //========================[3. Lấy lượt hành động của nhân vật]========================
            var battler = await TakeBattlerOfCurrentTurn(cts);

            //========================[4. Chọn mục tiêu ngẫu nhiên]========================
            var targets = GetRandomTargets(battler.actionTurnActorModel);
            if (targets.Count == 0)
            {
                return null; // Không có mục tiêu → bỏ lượt
            }

            //========================[5. Thực hiện hành động tấn công]========================
            Debug.Log(
                $"<color=yellow>Faction: {battler.actionTurnActorModel.Card.Battle.FactionIndex}" +
                $"\nSpeed: {battler.actionTurnActorModel.Card.Battle.Attributes[AttributeType.AttackSpeed]}" +
                $" | Action Point: {battler.actionTurnActorModel.Card.Battle.ActionPoint}</color>");

            await HandleBattlerAction(
                battler.actionTurnActorModel,
                battler.boardFactionPosition,
                targets,
                cts
            );
            
            Debug.Log(
                $"<color=yellow>Action Point After: {battler.boardFactionPosition.Card.Battle.ActionPoint}</color>");

            //========================[6. Kết thúc hành động]========================
            await UniTask.Yield();
            return null;
        }

        public async UniTask Melee(BoardFactionPosition actor, List<BoardFactionPosition> targets,
            CancellationTokenSource cts)
        {
            //========================[Camera Focus]===============\
            await PerformCameraFocus(actor.Card.transform, 7f, cts);
            //========================[Prepare Melee Attack]===============\
            Vector3 originalPosition = actor.Card.transform.position;

            //========================[Set parent]===============\
            var originalActorParent = actor.Card.transform.parent;
            actor.Card.transform.SetParent(_cardActingContainer, false);

            //========================[Hide vital bar]===============\
            actor.Card.Battle.Vital.Show(false);
            //========================[Cal attacker]======================\\
            var attackerResult = actor.Card.Battle.GetDamage();
            Debug.Log("Attacker: " + JsonConvert.SerializeObject(attackerResult));
            foreach (var target in targets)
            {
                //========================[Setup Attack]===============\
                var targetCard = target.Card.transform;
                Vector3 targetPosition = targetCard.position;
                Vector3 actorPosition = actor.Card.transform.position;

                //========================[Cal victim]===================================\\
                var victimResult = target.Card.Battle.OnTakeDamage(attackerResult.damage, actor.Card);
                Debug.Log("Victim:" + JsonConvert.SerializeObject(victimResult));

                //========================[Set parent]===============\
                var originalTargetParent = targetCard.parent;
                targetCard.transform.SetParent(_cardTargetContainer, false);

                //========================[Move to target]===============\
                float offsetY = actorPosition.y > targetPosition.y ? 1.0f : -1.0f;
                Vector3 attackPosition = new Vector3(targetPosition.x, targetPosition.y + offsetY, targetPosition.z);
                await actor.Card.transform.DOMove(attackPosition, 0.5f).SetEase(Ease.OutQuad)
                    .WithCancellation(cancellationToken: cts.Token);

                bool isActingDone = false;
                bool isShowFloatingEffect = false;

                //========================[Perform Melee Attack Animation]===============\
                _tweenAction = actor.Card.transform.DORotate(new Vector3(0, 0, offsetY < 0 ? 80 : -80), 0.2f)
                    .SetEase(Ease.InQuad).OnPlay(() =>
                        Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_Attack.ogg"))
                    .OnUpdate(async () =>
                    {
                        if (_tweenAction.ElapsedPercentage() >= 0.5f && !isShowFloatingEffect)
                        {
                            Global.Instance.Get<EffectManager>()
                                .ShowDamage(victimResult.aDamage, targetPosition);
                            Global.Instance.Get<EffectManager>()
                                .ShowDamageLog(attackerResult.logs.Concat(victimResult.logs).ToList(), targetPosition);
                            Global.Instance.Get<EffectManager>().ShowSlash(targetPosition);
                            var isDead = target.Card.Battle.OnTakeDamageLate();
                            if (isDead)
                            {
                                _actionTurnManager.SetDieActorTurnUI(target.Card.Battle.FactionIndex,
                                    target.Card.Battle.MemberIndex);
                            }

                            isShowFloatingEffect = true;

                            Vector3 takeHitPos = new Vector3(targetPosition.x, targetPosition.y - (offsetY / 2),
                                targetPosition.z);

                            //========================[Target act receive hit]===============\
                            await targetCard.transform.DOMove(takeHitPos, 0.3f)
                                .OnPlay(() => _cameraShake.Shake(2f, 3f, 0.2f))
                                .WithCancellation(cancellationToken: cts.Token);

                            isActingDone = true;
                        }
                    });

                await _tweenAction.WithCancellation(cancellationToken: cts.Token);
                await UniTask.WaitUntil(() => isActingDone, cancellationToken: cts.Token);

                //========================[Reset Rotation]===============\
                await actor.Card.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad)
                    .WithCancellation(cancellationToken: cts.Token);
                //========================[Return to Position]===============\
                await targetCard.transform.DOMove(targetPosition, 0.3f).WithCancellation(cancellationToken: cts.Token);

                //========================[Set parent]===============\
                targetCard.transform.SetParent(originalTargetParent, false);
            }

            //========================[Return to Position]===============\
            await actor.Card.transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InQuad)
                .WithCancellation(cancellationToken: cts.Token);
            //========================[Set parent]===============\
            actor.Card.transform.SetParent(originalActorParent, false);
            //========================[Show vital bar]===============\
            actor.Card.Battle.Vital.Show();
        }

        public async UniTask Ranged(BoardFactionPosition actor, List<BoardFactionPosition> targets,
            CancellationTokenSource cts)
        {
            //========================[Prepare Melee Attack]===============\
            Vector3 originalPosition = actor.Card.transform.position;

            //========================[Set parent]===============\
            var originalActorParent = actor.Card.transform.parent;
            actor.Card.transform.SetParent(_cardActingContainer, false);

            //========================[Hide vital bar]===============\
            actor.Card.Battle.Vital.Show(false);
            //========================[Cal attacker]======================\\
            var attackerResult = actor.Card.Battle.GetDamage();
            Debug.Log("Attacker: " + JsonConvert.SerializeObject(attackerResult));
            foreach (var target in targets)
            {
                //========================[Setup Attack]===============\
                var targetCard = target.Card.transform;
                Vector3 targetPosition = targetCard.position;
                Vector3 actorPosition = actor.Card.transform.position;

                //========================[Cal victim]===================================\\
                var victimResult = target.Card.Battle.OnTakeDamage(attackerResult.damage, actor.Card);
                Debug.Log("Victim:" + JsonConvert.SerializeObject(victimResult));

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
                await PerformCameraFocus(actor.Card.transform, 7f, cts);
                await actor.Card.transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.OutQuad)
                    .WithCancellation(cancellationToken: cts.Token);
                //

                _tweenAction = actor.Card.transform.DOMoveY(originalPosition.y + (offsetY / 3), 0.1f)
                    .SetEase(Ease.InQuad).OnPlay(() =>
                        Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_Attack.ogg"))
                    .OnUpdate(async () =>
                    {
                        if (_tweenAction.ElapsedPercentage() >= 0.5f && !isShowFloatingEffect)
                        {
                            isShowFloatingEffect = true;

                            PerformCameraReset(cts).Forget();
                            await Global.Instance.Get<EffectManager>()
                                .ShowBullet(actorPosition, targetPosition);
                            Global.Instance.Get<EffectManager>()
                                .ShowDamage(victimResult.aDamage, targetPosition);
                            Global.Instance.Get<EffectManager>()
                                .ShowDamageLog(attackerResult.logs.Concat(victimResult.logs).ToList(), targetPosition);
                            Global.Instance.Get<EffectManager>().ShowSlash(targetPosition);
                            target.Card.Battle.OnTakeDamageLate();
                            var isDead = target.Card.Battle.OnTakeDamageLate();
                            if (isDead)
                            {
                                _actionTurnManager.SetDieActorTurnUI(target.Card.Battle.FactionIndex,
                                    target.Card.Battle.MemberIndex);
                            }

                            Vector3 takeHitPos = new Vector3(targetPosition.x, targetPosition.y - (offsetY / 2),
                                targetPosition.z);

                            //========================[Target act receive hit]===============\
                            await targetCard.transform.DOMove(takeHitPos, 0.3f)
                                .OnPlay(() => _cameraShake.Shake(2f, 3f, 0.2f))
                                .WithCancellation(cancellationToken: cts.Token);

                            isActingDone = true;
                        }
                    });

                await _tweenAction.WithCancellation(cancellationToken: cts.Token);
                ;
                await UniTask.WaitUntil(() => isActingDone, cancellationToken: cts.Token);

                //========================[Reset Move]===============\
                await actor.Card.transform.DOMoveY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad)
                    .WithCancellation(cancellationToken: cts.Token);
                await actor.Card.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad)
                    .WithCancellation(cancellationToken: cts.Token);

                //========================[Return to Position]===============\
                await targetCard.transform.DOMove(targetPosition, 0.3f).WithCancellation(cancellationToken: cts.Token);

                //========================[Set parent]===============\
                targetCard.transform.SetParent(originalTargetParent, false);
            }

            //========================[Return to Position]===============\
            await actor.Card.transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InQuad)
                .WithCancellation(cancellationToken: cts.Token);
            //========================[Set parent]===============\
            actor.Card.transform.SetParent(originalActorParent, false);
            //========================[Show vital bar]===============\
            actor.Card.Battle.Vital.Show();
        }

        private async UniTask PerformCameraFocus(Transform target, float zoom, CancellationTokenSource cts)
        {
            await DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, zoom, 0.5f)
                .OnPlay(() => { _camera.Follow = target; })
                .SetEase(Ease.InOutSine)
                .WithCancellation(cancellationToken: cts.Token);
        }

        private async UniTask PerformCameraReset(CancellationTokenSource cts)
        {
            await DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, 10f, 0.5f)
                .OnPlay(() => { _camera.Follow = _transFormCameraCenterPoint; })
                .SetEase(Ease.InOutSine)
                .WithCancellation(cancellationToken: cts.Token);
        }
    }
}