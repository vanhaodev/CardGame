using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace World.Board
{
    public partial class ActionTurnManager : IDisposable
    {
        /// <summary>
        /// mặc định có 12 phần tử, ẩn phần tử không có
        /// </summary>
        [SerializeField] [BoxGroup("Display")] private List<ActorTurnOrderItemUI> _actorTurnUIs;

        [SerializeField] [BoxGroup("Display")] private ContentSizeFitter2 _sizeFitterActorTurnLayoutGroup;
        [SerializeField] [BoxGroup("Display")] private GameObject _objActorTurnUICurrentBorder;
        private CancellationTokenSource _ctsTurnUICurrentBorder;

        /// <summary>
        /// cập nhật danh sách battler được hành động ở round này theo thứ tự AP từ trái qua phải
        /// </summary>
        public void UpdateNewRoundActorTurnUI()
        {
            int i = 0;
            for (; i < ActionAvailableOrders.Count; i++)
            {
                var actor = ActionAvailableOrders[i];
                _actorTurnUIs[i].Setup(actor.Card.Battle.FactionIndex, actor.Card.Battle.MemberIndex, actor.Card.SpriteCharacter);
                _actorTurnUIs[i].Show();
            }

            for (; i < _actorTurnUIs.Count; i++)
            {
                _actorTurnUIs[i].Show(false);
            }
            _sizeFitterActorTurnLayoutGroup.UpdateSizeAnimation().Forget();
        }

        /// <summary>
        /// đánh dấu turn hiện tại của character này
        /// </summary>
        public async UniTask SetCurrentActorTurnUI()
        {
            var index = _actorTurnUIs.FindIndex(i => i.IsCurrent(CurrentTurn != null ? CurrentTurn.Card.Battle.FactionIndex : 0,
                CurrentTurn != null ? CurrentTurn.Card.Battle.MemberIndex : 0));
            _objActorTurnUICurrentBorder.SetActive(index != -1);
            if (index == -1)
            {
                return;
            }

            var currentUI = _actorTurnUIs[index];
            _ctsTurnUICurrentBorder = new CancellationTokenSource();
            var worldTargetPos = currentUI.transform.position;
            var parent = _objActorTurnUICurrentBorder.transform.parent;
            var localTargetPos = parent.InverseTransformPoint(worldTargetPos);

            var notCurrentUIs = new List<UniTask>();
            foreach (var ui in _actorTurnUIs)
            {
                if (ui == currentUI)
                {
                    var borderMove = _objActorTurnUICurrentBorder.transform
                        .DOLocalMove(localTargetPos, 0.2f)
                        .WithCancellation(cancellationToken: _ctsTurnUICurrentBorder.Token).AsAsyncUnitUniTask();
                    var currentScale = ui.transform.DOScale(new Vector3(1, 1, 1), 0.2f)
                        .WithCancellation(cancellationToken: _ctsTurnUICurrentBorder.Token).AsAsyncUnitUniTask();
                    await UniTask.WhenAll(borderMove, currentScale);
                }
                else
                {
                    var notCurrentScale = ui.transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.2f)
                        .WithCancellation(cancellationToken: _ctsTurnUICurrentBorder.Token).AsAsyncUnitUniTask();
                    notCurrentUIs.Add(notCurrentScale);
                }
            }

            await UniTask.WhenAll(notCurrentUIs);
        }

        /// <summary>
        /// Hiện thị vị trí này đã chết để người chơi nhìn ra
        /// </summary>
        /// <param name="factionIndex"></param>
        /// <param name="memberIndex"></param>
        public void SetDieActorTurnUI(int factionIndex, int memberIndex)
        {
            var actor = _actorTurnUIs.Find(i => i.IsThis(factionIndex, memberIndex));
            if (actor == null)
            {
                return;
            }

            actor.SetDieMask(true);
        }

        /// <summary>
        /// Hiện thị vị trí này đã sống lại để người chơi nhìn ra
        /// </summary>
        /// <param name="factionIndex"></param>
        /// <param name="memberIndex"></param>
        public void SetLiveActorTurnUI(int factionIndex, int memberIndex)
        {
            var actor = _actorTurnUIs.Find(i => i.IsThis(factionIndex, memberIndex));
            if (actor == null)
            {
                return;
            }

            actor.SetDieMask(false);
        }

        public void Dispose()
        {
            _ctsTurnUICurrentBorder?.Cancel();
            _ctsTurnUICurrentBorder?.Dispose();
        }
    }
}