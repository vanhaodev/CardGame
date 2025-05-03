using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Utils;
using World.TheCard;

namespace World.Board
{
    public class TargetSelectorController : MonoBehaviour
    {
        public byte AOEToLeftTargetCount;
        public byte AOEToRightTargetCount;
        [SerializeField] private GameObject _prefabTargetPointer;
        private DynamicObjectPool<GameObject> _pointerPool;
        private Board _board;
        [SerializeField] List<GameObject> _instantiatedPointers;

        private void Start()
        {
            _pointerPool = new DynamicObjectPool<GameObject>(
                createFunc: () =>
                {
                    var pointer = Instantiate(_prefabTargetPointer, transform);
                    if (!_instantiatedPointers.Contains(pointer))
                    {
                        _instantiatedPointers.Add(pointer);
                    }

                    return pointer;
                },
                resetAction: (pointer) => { pointer.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); }
            );
        }

        public void InitTargets(Board board)
        {
            _board = board;
            var battlers = board.GetAllFactions()
                .SelectMany(faction => faction.GetAllPositions())
                .Select(position => position.Card)
                .ToList();

            for (int i = 0; i < battlers.Count; i++)
            {
                battlers[i].EventOnTouch.Subscribe(OnTouch).AddTo(this);
            }
        }

        public void OnTouch(Card card)
        {
            foreach (var p in _instantiatedPointers)
            {
                _pointerPool.Put(p);
            }

            Debug.Log(card.Battle.FactionIndex + " | " + card.Battle.MemberIndex);
            var poiner = _pointerPool.Get();
            poiner.transform.position = card.transform.position;
        }
    }
}