using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.World.Gacha
{
    public class GachaCardResultManager : MonoBehaviour
    {
        [SerializeField] List<CardGacha> _cardGachas;
        [SerializeField] private Button _btnOpenAll;
        [SerializeField] private Button _btnClose;

        public void Show(List<GachaCardRewardModel> results)
        {
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
        }

        public void OpenAll()
        {
            foreach (var card in _cardGachas)
            {
                card.OnFlipOpen();
            }
        }

        private void OnOpen()
        {
            bool isAllCardsOpened = _cardGachas.TrueForAll(i => i.IsOpened());
            _btnClose.transform.parent.gameObject.SetActive(isAllCardsOpened);
            _btnOpenAll.transform.parent.gameObject.SetActive(!isAllCardsOpened);
        }
    }
}