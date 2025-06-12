using System.Collections.Generic;
using UnityEngine;

namespace Functions.World.Gacha
{
    public class GachaCardResultManager : MonoBehaviour
    {
        [SerializeField] List<CardGacha> _cardGachas;
        public void Show(List<GachaCardRewardModel> results)
        {
            int resultCount = results.Count;

            for (int i = 0; i < _cardGachas.Count; i++)
            {
                if (i < resultCount)
                {
                    _cardGachas[i].gameObject.SetActive(true);
                    if (results[i].Card != null)
                    {
                        _cardGachas[i].InitCard(results[i].Card);
                    }
                    else
                    {
                        _cardGachas[i].InitItem(results[i].ShardModel, results[i].Quantity);
                    }
                }
                else
                {
                    _cardGachas[i].gameObject.SetActive(false);
                }
            }
            gameObject.SetActive(true);
        }
    }
}