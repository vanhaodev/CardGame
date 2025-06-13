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
using Globals;
using UnityEngine.Serialization;
using Utils;

namespace Functions.World.Gacha
{
    public class GachaCardResultManager : MonoBehaviour
    {
        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteBackground;
        [BoxGroup("Tab")] [SerializeField] private Image _imageBackground;
        [SerializeField] private Image _imageFade;
        [SerializeField] private Transform _transformStartPos;
        [SerializeField] List<Vector3> _cardGachasOriginPositions;
        [SerializeField] List<Vector3> _cardGachasOriginScales;
        [SerializeField] List<CardGacha> _cardGachas;
        [SerializeField] private Button _btnOpenAll;
        [SerializeField] private Button _btnClose;
        [SerializeField] private List<RectTransform> _worldShakers;
        UIShaker _uiShaker;

        public void SetFadeSprite(Sprite sprite)
        {
            _imageFade.sprite = sprite;
        }

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
                _cardGachas[i].ShowShadow(false);
            }
        }

        private bool _isPlayingFallSound;

        private async UniTask AnimCardPosSequentialAsync()
        {
            float fallDuration = 0.4f;
            float fallScaleRatio = 0.4f; // nhỏ hơn gốc 60%
            float fallDelay = 0.3f; // delay giữa các lượt

            for (int i = 0; i < _cardGachas.Count; i++)
            {
                var card = _cardGachas[i];
                if (!card.gameObject.activeSelf) continue;
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
                cardTransform.DOLocalMove(targetPos, fallDuration).OnPlay(async () =>
                    {
                        if (!_isPlayingFallSound)
                        {
                            Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_GachaCardFall");
                            _isPlayingFallSound = true;
                        }

                        await UniTask.WaitForSeconds(fallDuration * 0.7f);
                        Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_GachaCardGround");
                    }).OnComplete(() => { _isPlayingFallSound = false; })
                    .SetEase(Ease.InQuad);

                cardTransform.DOScale(originScale, fallDuration)
                    .SetEase(Ease.InQuad).OnComplete(() => card.ShowShadow(true));

                //========================
                //Step: Delay nhẹ trước thẻ tiếp theo
                await UniTask.Delay(TimeSpan.FromSeconds(fallDelay));
            }
        }


        public async void Show(List<GachaCardRewardModel> results)
        {
            Global.Instance.Get<SoundManager>().StopSoundLoop(1);
            if (_imageBackground.sprite == null)
            {
                _uiShaker = new();
                var bg = await _assetRefSpriteBackground.AssetRef.LoadAssetAsync<Sprite>();
                _imageBackground.sprite = bg;
            }

            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            _imageFade.gameObject.SetActive(true);
            _imageFade.color = new Color(1, 1, 1, 0);
            Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_GachaCard");
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
            await UniTask.WaitForSeconds(0.4f);
            await _imageFade.DOFade(1, 0.3f);
            _uiShaker.StartWobble(_worldShakers, 10f, 1.2f);
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            await UniTask.WaitForSeconds(1f);
            await _imageFade.DOFade(0, 1).OnComplete(() => _imageFade.gameObject.SetActive(false));
            await AnimCardPosSequentialAsync();
            Global.Instance.Get<SoundManager>().PlaySoundLoop("BGM_GachaResultAmbient", 2);
            _btnOpenAll.transform.parent.gameObject.SetActive(true);
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

        private void OnDisable()
        {
            Global.Instance.Get<SoundManager>().PlaySoundLoop("BGM_Lobby", 1);
            Global.Instance.Get<SoundManager>().PlaySoundLoop("BGM_GachaResultAmbient", 2);
            _uiShaker.StopWobble();
        }

        public void Clear()
        {
            _imageBackground.sprite = null;
            _imageFade.gameObject.SetActive(false);
            _assetRefSpriteBackground.AssetRef.ReleaseAsset();
            _uiShaker = null;
        }
    }
}