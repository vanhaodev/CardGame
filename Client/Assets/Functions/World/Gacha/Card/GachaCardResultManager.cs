using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Functions.World.Gacha
{
    public class GachaCardResultManager : MonoBehaviour
    {
        [SerializeField] private Transform _transformStartPos;
        [SerializeField] List<Vector3> _cardGachasOriginPositions;
        [SerializeField] List<Vector3> _cardGachasOriginScales;
        [SerializeField] List<CardGacha> _cardGachas;
        [SerializeField] private Button _btnOpenAll;
        [SerializeField] private Button _btnClose;

        [Button]
        private void RefreshOriginPositions()
        {
            _cardGachasOriginPositions = new List<Vector3>(_cardGachas.Count);
            _cardGachasOriginScales = new List<Vector3>(_cardGachas.Count);
            for (int i = 0; i < _cardGachas.Count; i++)
            {
                _cardGachasOriginPositions.Add(_cardGachas[i].transform.localPosition);
                _cardGachasOriginScales.Add(_cardGachas[i].transform.localScale);
            }
        }

        private void ResetCardPos()
        {
            for (int i = 0; i < _cardGachas.Count; i++)
            {
                _cardGachas[i].transform.localPosition = _transformStartPos.localPosition;
                _cardGachas[i].transform.localScale = _cardGachasOriginScales[i];
            }
        }

        private async UniTask AnimCardPosSequentialAsync()
        {
            float fallDuration = 0.4f;
            float fallScaleRatio = 0.4f; // nhỏ hơn gốc 60%
            float fallDelay = 0.05f; // delay giữa các lượt

            for (int i = 0; i < _cardGachas.Count; i++)
            {
                var card = _cardGachas[i];
                var targetPos = _cardGachasOriginPositions[i];
                var originScale = _cardGachasOriginScales[i];
                var cardTransform = card.transform;

                //========================
                //Step: Hủy tween trước đó
                DOTween.Kill(cardTransform);

                //========================
                //Step: Reset trạng thái trước khi rơi
                cardTransform.localRotation = Quaternion.identity;
                cardTransform.localScale = originScale * fallScaleRatio;

                //========================
                //Step: Animate rơi + scale (không xoay)
                cardTransform.DOLocalMove(targetPos, fallDuration)
                    .SetEase(Ease.InQuad);

                cardTransform.DOScale(originScale, fallDuration)
                    .SetEase(Ease.InQuad);

                //========================
                //Step: Delay nhẹ trước thẻ tiếp theo
                await UniTask.Delay(TimeSpan.FromSeconds(fallDelay));
            }
        }


        public void Show(List<GachaCardRewardModel> results)
        {
            ResetCardPos();
            _btnClose.transform.parent.gameObject.SetActive(false);
            _btnOpenAll.transform.parent.gameObject.SetActive(false);
            int resultCount = results.Count;

            for (int i = 0; i < _cardGachas.Count; i++)
            {
                if (i < resultCount)
                {
                    _cardGachas[i].gameObject.SetActive(true);
                    if (results[i].Card != null)
                    {
                        _cardGachas[i].InitCard(results[i].Card, OnOpen);
                    }
                    else
                    {
                        _cardGachas[i].InitItem(results[i].ShardModel, results[i].Quantity, OnOpen);
                    }
                }
                else
                {
                    _cardGachas[i].gameObject.SetActive(false);
                }
            }

            gameObject.SetActive(true);
            _btnOpenAll.transform.parent.gameObject.SetActive(true);
            AnimCardPosSequentialAsync();
        }

        public void OpenAll()
        {
            foreach (var card in _cardGachas)
            {
                if (!card.gameObject.activeSelf || card.IsOpened()) continue;
                card.OnFlipOpen();
            }
        }

        private void OnOpen()
        {
            // Chỉ duyệt các object đang active
            bool isAllCardsOpened = _cardGachas
                .Where(i => i.gameObject.activeSelf)
                .All(i => i.IsOpened());

            _btnClose.transform.parent.gameObject.SetActive(isAllCardsOpened);
            _btnOpenAll.transform.parent.gameObject.SetActive(!isAllCardsOpened);
        }
    }
}