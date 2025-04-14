using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World.Board
{
    public partial class ActionTurnManager : IDisposable
    {
        /// <summary>
        /// mặc định có 12 phần tử, ẩn phần tử không có
        /// </summary>
        [SerializeField] [BoxGroup("Display")] private List<ActorTurnUI> _actorTurnUIs;

        [SerializeField] [BoxGroup("Display")] private GameObject _objActorTurnUICurrentBorder;
        private CancellationTokenSource _ctsTurnUICurrentBorder;

        public void UpdateNewRoundActorTurnUI()
        {
            int i = 0;
            for (; i < ActionAvailableOrders.Count; i++)
            {
                var actor = ActionAvailableOrders[i];
                _actorTurnUIs[i].Setup(actor.FactionIndex, actor.MemberIndex, actor.Card.SpriteCharacter);
                _actorTurnUIs[i].Show();
            }

            for (; i < _actorTurnUIs.Count; i++)
            {
                _actorTurnUIs[i].Show(false);
            }
        }

        public void SetCurrentActorTurnUI()
        {
            var index = _actorTurnUIs.FindIndex(i => i.IsCurrent(CurrentTurn != null ? CurrentTurn.FactionIndex : 0,
                CurrentTurn != null ? CurrentTurn.MemberIndex : 0));
            _objActorTurnUICurrentBorder.SetActive(index != -1);
            if (index == -1)
            {
                return;
            }

            var currentUI = _actorTurnUIs[index];
            _objActorTurnUICurrentBorder.transform.DOMove(currentUI.transform.position, 2f)
                .WithCancellation(cancellationToken: _ctsTurnUICurrentBorder.Token);
        }

        public void Dispose()
        {
            _ctsTurnUICurrentBorder?.Cancel();
            _ctsTurnUICurrentBorder?.Dispose();
        }
    }
}