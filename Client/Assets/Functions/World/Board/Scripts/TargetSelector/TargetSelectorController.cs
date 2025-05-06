using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Globals;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] List<Card> _selectingCards;
        private CancellationTokenSource _ctsOnTouch;

        private void Start()
        {
            _pointerPool = new DynamicObjectPool<GameObject>(
                createFunc: () =>
                {
                    var pointer = Instantiate(_prefabTargetPointer, transform);
                    pointer.SetActive(false);
                    _instantiatedPointers.Add(pointer);

                    return pointer;
                },
                resetAction: (pointer) => { pointer.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); }
            );
        }

        public void Show(bool isShow = true)
        {
            gameObject.SetActive(isShow);
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


        public async void OnTouch(Card card)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            if (card.Battle.IsDead) return;
            _ctsOnTouch?.Cancel();
            _ctsOnTouch = new CancellationTokenSource();
            foreach (var instantiatedPointer in _instantiatedPointers)
            {
                instantiatedPointer.gameObject.SetActive(false);
            }

            //
            // Lấy danh sách các card của faction và sắp xếp theo MemberIndex
            var factionCards = _board.GetFactionByIndex(card.Battle.FactionIndex).GetAllPositions()
                .OrderBy(c => c.MemberIndex)
                .ToList();

            // Tìm index của card center trong danh sách
            int centerIndex = factionCards.FindIndex(c => c.MemberIndex == card.Battle.MemberIndex);

            if (centerIndex == -1)
            {
                Debug.LogWarning("Card not found in faction.");
                return;
            }

            Debug.Log("FactionIndex " + card.Battle.FactionIndex + " | MemberIndex " + card.Battle.MemberIndex +
                      " | Center = " + centerIndex);

            // Xác định các card mục tiêu (card center sẽ luôn được chọn)
            _selectingCards = new();
            // Thêm card center vào đúng giữa
            _selectingCards.Add(factionCards[centerIndex].Card);
            Debug.Log("Add center index " + centerIndex);
            // Lan ra dần đều hai bên
            for (int i = 1; i <= Mathf.Max(AOEToLeftTargetCount, AOEToRightTargetCount); i++)
            {
                // Bên trái
                int leftIndex = centerIndex - i;
                if (i <= AOEToLeftTargetCount && leftIndex >= 0)
                {
                    if (!factionCards[leftIndex].Card.Battle.IsDead)
                        _selectingCards.Add(factionCards[leftIndex].Card);
                }

                // Bên phải
                int rightIndex = centerIndex + i;
                if (i <= AOEToRightTargetCount && rightIndex < factionCards.Count)
                {
                    if (!factionCards[rightIndex].Card.Battle.IsDead)
                        _selectingCards.Add(factionCards[rightIndex].Card);
                }
            }

            Debug.Log("Target count " + _selectingCards.Count);
            // Đảm bảo đủ pointer
            while (_instantiatedPointers.Count < _selectingCards.Count)
            {
                var newPointer = _pointerPool.Get();
            }

            // Lưu danh sách instance id của các pointer đã được hiển thị
            HashSet<int> activePointers = new();
            // List<UniTask> tasks = new();
            // Cập nhật vị trí pointer và trạng thái hiển thị
            for (int i = 0; i < _selectingCards.Count; i++)
            {
                var pointer = _instantiatedPointers[i];
                pointer.name =
                    $"Pointer {_selectingCards[i].Battle.FactionIndex} | {_selectingCards[i].Battle.MemberIndex}";
                // Lưu instance id của pointer đã được hiển thị
                activePointers.Add(pointer.GetInstanceID());

                // Cập nhật vị trí pointer
                pointer.transform.position = _selectingCards[i].transform.position;
                // Hiển thị pointer nếu mục tiêu có trong danh sách
                pointer.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                pointer.gameObject.SetActive(true);

                if (i == 0)
                {
                    Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_TargetSelect");
                    await pointer.transform.DOScale(0.7f, 0.2f).WithCancellation(cancellationToken: _ctsOnTouch.Token);
                }
                else
                {
                    if (_selectingCards[i].Battle.MemberIndex < card.Battle.MemberIndex)
                    {
                        Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_TargetSelectLeft");
                    }
                    else
                    {
                        Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_TargetSelectRight");
                    }

                    await pointer.transform.DOScale(0.45f, 0.2f).WithCancellation(cancellationToken: _ctsOnTouch.Token);
                    //await UniTask.WaitForSeconds(0.15f, cancellationToken: _ctsOnTouch.Token);
                }
            }

            // Ẩn các pointer thừa (nếu có) và loại trừ các pointer đã được hiển thị
            for (int i = 0; i < _instantiatedPointers.Count; i++)
            {
                var pointer = _instantiatedPointers[i];

                // Nếu pointer chưa được hiển thị (instance id không có trong activePointers), ẩn nó
                if (!activePointers.Contains(pointer.GetInstanceID()))
                {
                    pointer.gameObject.SetActive(false);
                }
            }
        }

        public List<BoardFactionPosition> GetSelectingFactions()
        {
            List<BoardFactionPosition> selecting = new List<BoardFactionPosition>();
            foreach (var s in _selectingCards)
            {
                var bfp = _board.GetFactionByIndex(s.Battle.FactionIndex)
                    .GetPositionByIndex(s.Battle.MemberIndex);
                selecting.Add(bfp);
            }

            return selecting;
        }
    }
}